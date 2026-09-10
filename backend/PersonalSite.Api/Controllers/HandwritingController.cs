using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Models;
using PersonalSite.Api.Services;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HandwritingController : ControllerBase
{
    private const long MaxImageBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png", "image/webp" };

    private readonly AiServiceClient _aiServiceClient;

    public HandwritingController(AiServiceClient aiServiceClient)
    {
        _aiServiceClient = aiServiceClient;
    }

    [HttpPost("extract")]
    [RequestSizeLimit(MaxImageBytes)]
    public async Task<ActionResult<ApiResponse<HandwritingExtractResult>>> Extract(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return Ok(ApiResponse<HandwritingExtractResult>.Error("请选择一张纸张照片"));
        if (file.Length > MaxImageBytes)
            return Ok(ApiResponse<HandwritingExtractResult>.Error("图片不要超过 10MB"));
        if (!AllowedContentTypes.Contains(file.ContentType))
            return Ok(ApiResponse<HandwritingExtractResult>.Error("请使用 JPG、PNG 或 WebP 图片"));

        await using var imageStream = file.OpenReadStream();
        using var memory = new MemoryStream();
        await imageStream.CopyToAsync(memory, cancellationToken);

        try
        {
            var result = await _aiServiceClient.ExtractHandwritingAsync(
                file.ContentType,
                Convert.ToBase64String(memory.ToArray()),
                cancellationToken);
            return Ok(ApiResponse<HandwritingExtractResult>.Success(result));
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Ok(ApiResponse<HandwritingExtractResult>.Error("纸张识别超时，请再试一次"));
        }
        catch (HttpRequestException)
        {
            return Ok(ApiResponse<HandwritingExtractResult>.Error("暂时无法连接纸张识别服务"));
        }
        catch (InvalidOperationException ex)
        {
            return Ok(ApiResponse<HandwritingExtractResult>.Error(ex.Message));
        }
    }
}
