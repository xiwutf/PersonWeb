using Aliyun.OSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public MediaController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(accessKeyId) || string.IsNullOrEmpty(accessKeySecret))
        {
             return Ok(ApiResponse<MediaFile>.Error("OSS 配置不完整"));
        }

        try
        {
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            
            // 生成文件名: upload/yyyyMMdd/guid.ext
            var ext = Path.GetExtension(file.FileName).ToLower();
            var objectName = $"upload/{DateTime.Now:yyyyMMdd}/{Guid.NewGuid()}{ext}";

            using var stream = file.OpenReadStream();
            client.PutObject(bucketName, objectName, stream);

            var url = !string.IsNullOrEmpty(domain) 
                ? $"{domain}/{objectName}" 
                : $"https://{bucketName}.{endpoint}/{objectName}";

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
}
