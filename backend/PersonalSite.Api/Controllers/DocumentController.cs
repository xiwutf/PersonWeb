using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Models.Dto;
using PersonalSite.Api.Services;
using System.Text.Json;
using DocumentChunkDto = PersonalSite.Api.Models.Dto.DocumentChunkDto;
using DocumentQueryResponseDto = PersonalSite.Api.Models.Dto.DocumentQueryResponseDto;
using RelevantChunkDto = PersonalSite.Api.Models.Dto.RelevantChunkDto;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 文档知识管家 Agent 控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AiServiceClient _aiServiceClient;
    private readonly ILogger<DocumentController> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DocumentController(
        AppDbContext context,
        AiServiceClient aiServiceClient,
        ILogger<DocumentController> logger,
        IWebHostEnvironment env,
        IServiceScopeFactory serviceScopeFactory)
    {
        _context = context;
        _aiServiceClient = aiServiceClient;
        _logger = logger;
        _env = env;
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// 上传文档
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<DocumentListItemDto>>> UploadDocument(
        [FromForm] IFormFile file,
        [FromForm] string? title = null,
        [FromForm] string? userId = null)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return Ok(ApiResponse<DocumentListItemDto>.Error("请选择要上传的文件", 400));
            }

            // 验证文件类型
            var allowedExtensions = new[] { ".pdf", ".docx", ".txt", ".md" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return Ok(ApiResponse<DocumentListItemDto>.Error(
                    $"不支持的文件类型。支持的类型: {string.Join(", ", allowedExtensions)}", 400));
            }

            // 创建文档记录
            var document = new Document
            {
                Title = title ?? Path.GetFileNameWithoutExtension(file.FileName),
                FileName = file.FileName,
                FileType = fileExtension.TrimStart('.'),
                FileSize = file.Length,
                Status = "pending",
                UserId = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // 保存文件
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads", "documents");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var localFilePath = Path.Combine(uploadsFolder, fileName);
            // 使用绝对路径，确保 Python Agent 可以访问
            document.FilePath = Path.GetFullPath(localFilePath);

            using (var stream = new FileStream(localFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            _logger.LogInformation("文档上传成功: DocumentId={DocumentId}, FileName={FileName}",
                document.Id, document.FileName);

            // 异步处理文档（不阻塞响应）
            // 使用 Task.Run 在后台线程处理，但需要创建新的服务作用域
            var documentId = document.Id;
            var finalFilePath = document.FilePath;
            var finalFileType = document.FileType;
            var finalUserId = document.UserId ?? "default";
            var scopeFactory = _serviceScopeFactory;
            
            _ = Task.Run(async () =>
            {
                // 在后台任务中，需要创建新的服务作用域来访问 DbContext
                using var scope = scopeFactory.CreateScope();
                var scopedContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var scopedAiClient = scope.ServiceProvider.GetRequiredService<AiServiceClient>();
                var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<DocumentController>>();
                
                try
                {
                    await ProcessDocumentAsyncBackground(documentId, finalFilePath, finalFileType, finalUserId, scopedContext, scopedAiClient, scopedLogger);
                }
                catch (Exception ex)
                {
                    scopedLogger.LogError(ex, "异步处理文档失败: DocumentId={DocumentId}", documentId);
                }
            });

            var dto = new DocumentListItemDto
            {
                Id = document.Id,
                Title = document.Title,
                FileName = document.FileName,
                FileType = document.FileType,
                FileSize = document.FileSize,
                Status = document.Status,
                TotalChunks = document.TotalChunks,
                CreatedAt = document.CreatedAt,
                ProcessedAt = document.ProcessedAt
            };

            return Ok(ApiResponse<DocumentListItemDto>.Success(dto, "文档上传成功，正在处理中..."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "上传文档失败");
            return Ok(ApiResponse<DocumentListItemDto>.Error($"上传失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取文档列表
    /// </summary>
    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<object>>> GetDocumentList(
        [FromQuery] string? userId = null,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = _context.Documents.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(d => d.UserId == userId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(d => d.Status == status);
            }

            var total = await query.CountAsync();
            var documents = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DocumentListItemDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    FileName = d.FileName,
                    FileType = d.FileType,
                    FileSize = d.FileSize,
                    Status = d.Status,
                    Summary = d.Summary,
                    TotalChunks = d.TotalChunks,
                    ErrorMessage = d.ErrorMessage,
                    CreatedAt = d.CreatedAt,
                    ProcessedAt = d.ProcessedAt
                })
                .ToListAsync();

            return Ok(ApiResponse.Success(new { Total = total, List = documents }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取文档列表失败");
            return Ok(ApiResponse.Error($"获取列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取文档详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DocumentDetailDto>>> GetDocumentDetail(long id)
    {
        try
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return Ok(ApiResponse<DocumentDetailDto>.Error("文档不存在", 404));
            }

            var dto = new DocumentDetailDto
            {
                Id = document.Id,
                Title = document.Title,
                FileName = document.FileName,
                FilePath = document.FilePath,
                FileType = document.FileType,
                FileSize = document.FileSize,
                Status = document.Status,
                Summary = document.Summary,
                KnowledgeStructure = document.KnowledgeStructure,
                TotalChunks = document.TotalChunks,
                UserId = document.UserId,
                ErrorMessage = document.ErrorMessage,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
                ProcessedAt = document.ProcessedAt
            };

            return Ok(ApiResponse<DocumentDetailDto>.Success(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取文档详情失败: DocumentId={DocumentId}", id);
            return Ok(ApiResponse<DocumentDetailDto>.Error($"获取详情失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取文档分段列表
    /// </summary>
    [HttpGet("{id}/chunks")]
    public async Task<ActionResult<ApiResponse<List<DocumentChunkDto>>>> GetDocumentChunks(long id)
    {
        try
        {
            var chunks = await _context.DocumentChunks
                .Where(c => c.DocumentId == id)
                .OrderBy(c => c.ChunkIndex)
                .Select(c => new DocumentChunkDto
                {
                    Id = c.Id,
                    ChunkIndex = c.ChunkIndex,
                    Content = c.Content,
                    Summary = c.Summary,
                    Metadata = c.Metadata,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<DocumentChunkDto>>.Success(chunks));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取文档分段列表失败: DocumentId={DocumentId}", id);
            return Ok(ApiResponse<List<DocumentChunkDto>>.Error($"获取分段列表失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除文档
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteDocument(long id)
    {
        try
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return Ok(ApiResponse.Error("文档不存在", 404));
            }

            // 删除文件
            if (System.IO.File.Exists(document.FilePath))
            {
                System.IO.File.Delete(document.FilePath);
            }

            // 删除数据库记录（级联删除分段和问答记录）
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            _logger.LogInformation("文档删除成功: DocumentId={DocumentId}", id);

            return Ok(ApiResponse.Success(null, "删除成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除文档失败: DocumentId={DocumentId}", id);
            return Ok(ApiResponse.Error($"删除失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 重试处理文档（用于失败或待处理的文档）
    /// </summary>
    [HttpPost("{id}/retry")]
    public async Task<ActionResult<ApiResponse>> RetryProcessDocument(long id)
    {
        try
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return Ok(ApiResponse.Error("文档不存在", 404));
            }

            // 重置状态为 pending
            document.Status = "pending";
            document.ErrorMessage = null;
            document.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            _logger.LogInformation("重试处理文档: DocumentId={DocumentId}", id);

            // 异步处理文档
            var documentId = document.Id;
            var finalFilePath = document.FilePath;
            var finalFileType = document.FileType;
            var finalUserId = document.UserId ?? "default";
            var scopeFactory = _serviceScopeFactory;
            
            _ = Task.Run(async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var scopedContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var scopedAiClient = scope.ServiceProvider.GetRequiredService<AiServiceClient>();
                var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<DocumentController>>();
                
                try
                {
                    await ProcessDocumentAsyncBackground(documentId, finalFilePath, finalFileType, finalUserId, scopedContext, scopedAiClient, scopedLogger);
                }
                catch (Exception ex)
                {
                    scopedLogger.LogError(ex, "重试处理文档失败: DocumentId={DocumentId}", documentId);
                }
            });

            return Ok(ApiResponse.Success(null, "已开始重新处理文档"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重试处理文档失败: DocumentId={DocumentId}", id);
            return Ok(ApiResponse.Error($"重试失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 对单个文档进行问答
    /// </summary>
    [HttpPost("{id}/query")]
    public async Task<ActionResult<ApiResponse<DocumentQueryResponseDto>>> QueryDocument(
        long id,
        [FromBody] DocumentQueryRequest request)
    {
        try
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return Ok(ApiResponse<DocumentQueryResponseDto>.Error("文档不存在", 404));
            }

            if (document.Status != "completed")
            {
                return Ok(ApiResponse<DocumentQueryResponseDto>.Error(
                    "文档尚未处理完成，请稍后再试", 400));
            }

            // 调用 Python Agent 进行问答
            var queryResult = await _aiServiceClient.QueryDocumentAsync(
                documentId: id.ToString(),
                userId: request.UserId ?? "default",
                query: request.Query,
                topK: request.TopK
            );

            // 保存问答历史
            var documentQuery = new DocumentQuery
            {
                DocumentId = id,
                UserId = request.UserId,
                Query = request.Query,
                Answer = queryResult.Answer,
                RelevantChunks = JsonSerializer.Serialize(
                    queryResult.RelevantChunks.Select(c => c.ChunkId).ToList()),
                Confidence = queryResult.Confidence,
                CreatedAt = DateTime.Now
            };
            _context.DocumentQueries.Add(documentQuery);
            await _context.SaveChangesAsync();

            var response = new DocumentQueryResponseDto
            {
                Answer = queryResult.Answer,
                RelevantChunks = queryResult.RelevantChunks.Select(c => new RelevantChunkDto
                {
                    ChunkId = c.ChunkId,
                    ChunkIndex = c.ChunkIndex,
                    Content = c.Content,
                    Summary = c.Summary,
                    Score = c.Score
                }).ToList(),
                Confidence = queryResult.Confidence
            };

            return Ok(ApiResponse<DocumentQueryResponseDto>.Success(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "文档问答失败: DocumentId={DocumentId}", id);
            return Ok(ApiResponse<DocumentQueryResponseDto>.Error($"问答失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 异步处理文档（调用 Python Agent）- 后台任务版本
    /// </summary>
    private async Task ProcessDocumentAsyncBackground(
        long documentId,
        string filePath,
        string fileType,
        string userId,
        AppDbContext context,
        AiServiceClient aiServiceClient,
        ILogger<DocumentController> logger)
    {
        try
        {
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
            {
                logger.LogWarning("文档不存在: DocumentId={DocumentId}", documentId);
                return;
            }

            // 更新状态为处理中
            document.Status = "processing";
            await context.SaveChangesAsync();

            logger.LogInformation("开始处理文档: DocumentId={DocumentId}, FilePath={FilePath}", documentId, filePath);

            // 调用 Python Agent 处理文档
            var processResult = await aiServiceClient.ProcessDocumentAsync(
                documentId: documentId.ToString(),
                filePath: filePath,
                fileType: fileType,
                userId: userId
            );

            logger.LogInformation("Python Agent 处理完成，开始更新数据库: DocumentId={DocumentId}", documentId);

            // 重新获取文档（确保是最新的）
            document = await context.Documents.FindAsync(documentId);
            if (document == null)
            {
                logger.LogWarning("文档在处理过程中被删除: DocumentId={DocumentId}", documentId);
                return;
            }

            // 更新文档信息
            document.Status = "completed";
            document.Summary = processResult.Summary;
            document.KnowledgeStructure = processResult.KnowledgeStructure;
            document.TotalChunks = processResult.TotalChunks;
            document.ProcessedAt = DateTime.Now;
            document.UpdatedAt = DateTime.Now;

            // 保存分段
            foreach (var chunk in processResult.Chunks)
            {
                // 将 JsonElement 转换为 JSON 字符串
                string? metadataJson = null;
                if (chunk.Metadata.HasValue && chunk.Metadata.Value.ValueKind != System.Text.Json.JsonValueKind.Null)
                {
                    metadataJson = chunk.Metadata.Value.GetRawText();
                }
                
                var documentChunk = new DocumentChunk
                {
                    DocumentId = documentId,
                    ChunkIndex = chunk.Index,
                    Content = chunk.Content,
                    Summary = chunk.Summary,
                    Metadata = metadataJson,
                    VectorId = chunk.VectorId,
                    CreatedAt = DateTime.Now
                };
                context.DocumentChunks.Add(documentChunk);
            }

            await context.SaveChangesAsync();

            logger.LogInformation("文档处理完成: DocumentId={DocumentId}, Chunks={Chunks}",
                documentId, processResult.TotalChunks);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "处理文档失败: DocumentId={DocumentId}", documentId);

            // 更新状态为失败
            try
            {
                var document = await context.Documents.FindAsync(documentId);
                if (document != null)
                {
                    document.Status = "failed";
                    document.ErrorMessage = ex.Message.Length > 1000 ? ex.Message.Substring(0, 1000) : ex.Message;
                    document.UpdatedAt = DateTime.Now;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception saveEx)
            {
                logger.LogError(saveEx, "更新文档失败状态时出错: DocumentId={DocumentId}", documentId);
            }
        }
    }
}

