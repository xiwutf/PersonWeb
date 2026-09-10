using Aliyun.OSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Security.Claims;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public MediaController(
        AppDbContext context,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        _environment = environment;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost("upload")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<MediaFile>>> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Ok(ApiResponse<MediaFile>.Error("请选择文件"));
        }

        var endpoint = _configuration["Aliyun:OssEndpoint"];
        var accessKeyId = _configuration["Aliyun:AccessKeyId"];
        var accessKeySecret = _configuration["Aliyun:AccessKeySecret"];
        var bucketName = _configuration["Aliyun:BucketName"];
        var domain = _configuration["Aliyun:Domain"]; // 自定义域名或 OSS 域名

        try
        {
            // 生成文件名: upload/yyyyMMdd/guid.ext
            var ext = Path.GetExtension(file.FileName).ToLower();
            var objectName = $"upload/{DateTime.Now:yyyyMMdd}/{Guid.NewGuid()}{ext}";
            string url;

            if (!string.IsNullOrWhiteSpace(endpoint)
                && !string.IsNullOrWhiteSpace(accessKeyId)
                && !string.IsNullOrWhiteSpace(accessKeySecret)
                && !string.IsNullOrWhiteSpace(bucketName))
            {
                var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
                using var stream = file.OpenReadStream();
                client.PutObject(bucketName, objectName, stream);
                url = !string.IsNullOrEmpty(domain)
                    ? $"{domain.TrimEnd('/')}/{objectName}"
                    : $"https://{bucketName}.{endpoint}/{objectName}";
            }
            else
            {
                var webRoot = _environment.WebRootPath
                    ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                var relativePath = objectName.Replace('/', Path.DirectorySeparatorChar);
                var targetPath = Path.Combine(webRoot, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                await using var stream = System.IO.File.Create(targetPath);
                await file.CopyToAsync(stream);
                url = $"{Request.Scheme}://{Request.Host}/{objectName}";
            }

            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var mediaFile = new MediaFile
            {
                FileName = file.FileName,
                FileType = file.ContentType,
                Size = file.Length,
                Url = url,
                UploaderId = userId == 0 ? null : userId,
                CreatedAt = DateTime.Now
            };

            _context.MediaFiles.Add(mediaFile);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<MediaFile>.Success(mediaFile));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse<MediaFile>.Error($"上传失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 媒体文件列表
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _context.MediaFiles.AsQueryable();
        var total = query.Count();
        var list = query.OrderByDescending(x => x.CreatedAt)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

        return Ok(ApiResponse.Success(new { Total = total, List = list }));
    }

    /// <summary>
    /// 删除当前用户上传的媒体文件。
    /// </summary>
    [HttpDelete("upload")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> DeleteUpload([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Ok(ApiResponse.Error("图片地址不能为空"));
        }

        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var mediaFile = await _context.MediaFiles.FirstOrDefaultAsync(item =>
            item.Url == url && item.UploaderId == userId);
        if (mediaFile == null)
        {
            return Ok(ApiResponse.Error("没有找到可删除的图片"));
        }

        try
        {
            var endpoint = _configuration["Aliyun:OssEndpoint"];
            var accessKeyId = _configuration["Aliyun:AccessKeyId"];
            var accessKeySecret = _configuration["Aliyun:AccessKeySecret"];
            var bucketName = _configuration["Aliyun:BucketName"];

            if (url.Contains("/upload/", StringComparison.OrdinalIgnoreCase)
                && Uri.TryCreate(url, UriKind.Absolute, out var localUri)
                && (localUri.IsLoopback || localUri.Host.Equals(Request.Host.Host, StringComparison.OrdinalIgnoreCase)))
            {
                var webRoot = _environment.WebRootPath
                    ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                var uploadRoot = Path.GetFullPath(Path.Combine(webRoot, "upload"));
                var relativePath = localUri.AbsolutePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var targetPath = Path.GetFullPath(Path.Combine(webRoot, relativePath));
                if (targetPath.StartsWith(uploadRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                    && System.IO.File.Exists(targetPath))
                {
                    System.IO.File.Delete(targetPath);
                }
            }
            else if (!string.IsNullOrWhiteSpace(endpoint)
                && !string.IsNullOrWhiteSpace(accessKeyId)
                && !string.IsNullOrWhiteSpace(accessKeySecret)
                && !string.IsNullOrWhiteSpace(bucketName))
            {
                var objectName = new Uri(url).AbsolutePath.TrimStart('/');
                new OssClient(endpoint, accessKeyId, accessKeySecret).DeleteObject(bucketName, objectName);
            }

            _context.MediaFiles.Remove(mediaFile);
            await _context.SaveChangesAsync();
            return Ok(ApiResponse.Success(message: "图片已删除"));
        }
        catch
        {
            return Ok(ApiResponse.Error("图片删除失败，请稍后重试"));
        }
    }
}
