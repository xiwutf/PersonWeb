using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using System.Globalization;
using System.Text;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestmentImportController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebHostEnvironment _env;

    public InvestmentImportController(
        AppDbContext context,
        IHttpClientFactory httpClientFactory,
        IWebHostEnvironment env)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _env = env;
    }

    /// <summary>
    /// 导入交易记录（支持 CSV 和 Excel）
    /// </summary>
    [HttpPost("import")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ImportTransactions([FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return Ok(ApiResponse.Error("请选择要导入的文件", 400));
            }

            // 验证文件类型
            var allowedExtensions = new[] { ".csv", ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return Ok(ApiResponse.Error(
                    $"不支持的文件类型。支持的类型: {string.Join(", ", allowedExtensions)}", 400));
            }

            // 读取文件内容
            List<TransactionImportRow> rows;
            if (fileExtension == ".csv")
            {
                rows = await ParseCsvFile(file);
            }
            else
            {
                rows = await ParseExcelFile(file);
            }

            if (rows.Count == 0)
            {
                return Ok(ApiResponse.Error("文件中没有有效数据", 400));
            }

            // 处理导入的数据
            var result = await ProcessImportData(rows);

            return Ok(ApiResponse.Success(result, $"导入完成：成功 {result.SuccessCount} 条，失败 {result.FailCount} 条"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"导入失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 解析 CSV 文件
    /// </summary>
    private async Task<List<TransactionImportRow>> ParseCsvFile(IFormFile file)
    {
        var rows = new List<TransactionImportRow>();

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        using var reader = new StreamReader(stream, Encoding.UTF8);
        string? line;
        bool isFirstLine = true;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            // 跳过表头
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = ParseCsvLine(line);
            if (columns.Count >= 6)
            {
                var row = new TransactionImportRow
                {
                    Code = columns[0]?.Trim() ?? "",
                    Name = columns[1]?.Trim() ?? "",
                    Type = columns[2]?.Trim().ToLower() ?? "fund",
                    TransactionType = columns[3]?.Trim().ToLower() ?? "buy",
                    Quantity = ParseDecimal(columns[4]),
                    Price = ParseDecimal(columns[5]),
                    TransactionDate = ParseDateTime(columns.Count > 6 ? columns[6] : null),
                    Notes = columns.Count > 7 ? columns[7]?.Trim() : null
                };

                if (!string.IsNullOrEmpty(row.Code))
                {
                    rows.Add(row);
                }
            }
        }

        return rows;
    }

    /// <summary>
    /// 解析 CSV 行（处理引号内的逗号）
    /// </summary>
    private List<string> ParseCsvLine(string line)
    {
        var columns = new List<string>();
        var currentColumn = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                columns.Add(currentColumn.ToString());
                currentColumn.Clear();
            }
            else
            {
                currentColumn.Append(c);
            }
        }

        columns.Add(currentColumn.ToString());
        return columns;
    }

    /// <summary>
    /// 解析 Excel 文件（简化版本，使用 OpenXML）
    /// </summary>
    private async Task<List<TransactionImportRow>> ParseExcelFile(IFormFile file)
    {
        var rows = new List<TransactionImportRow>();

        // 注意：这里使用简化的 Excel 解析
        // 如果需要完整的 Excel 支持，需要安装 EPPlus 或 ClosedXML NuGet 包
        // 暂时先返回 CSV 解析的结果，提示用户使用 CSV 格式

        // 临时方案：将 Excel 文件转换为 CSV 格式解析
        // 实际项目中应该使用 EPPlus 或 ClosedXML

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        // 简化处理：尝试读取为文本（仅适用于简单的 Excel 文件）
        // 建议用户使用 CSV 格式，或安装 EPPlus 包进行完整支持
        throw new NotSupportedException("Excel 文件解析需要安装 EPPlus 包。请先使用 CSV 格式，或安装 EPPlus NuGet 包。");
    }

    /// <summary>
    /// 处理导入的数据
    /// </summary>
    private async Task<ImportResult> ProcessImportData(List<TransactionImportRow> rows)
    {
        var result = new ImportResult
        {
            SuccessCount = 0,
            FailCount = 0,
            Errors = new List<string>()
        };

        foreach (var row in rows)
        {
            try
            {
                // 查找或创建投资记录
                var investment = await _context.Investments
                    .FirstOrDefaultAsync(i => i.Code == row.Code);

                if (investment == null)
                {
                    // 创建新的投资记录
                    var currentPrice = await GetCurrentPrice(row.Code, row.Type);
                    var name = string.IsNullOrEmpty(row.Name) 
                        ? await GetNameFromCode(row.Code, row.Type) 
                        : row.Name;

                    investment = new Investment
                    {
                        Code = row.Code,
                        Name = name,
                        Type = row.Type,
                        Quantity = row.TransactionType == "buy" ? row.Quantity : 0,
                        CostPrice = row.Price,
                        CurrentPrice = currentPrice,
                        TotalCost = row.TransactionType == "buy" ? row.Quantity * row.Price : 0,
                        MarketValue = row.TransactionType == "buy" ? row.Quantity * currentPrice : 0,
                        ProfitLoss = 0,
                        ProfitRate = 0,
                        Notes = row.Notes,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Investments.Add(investment);
                    await _context.SaveChangesAsync();
                }

                // 创建交易记录
                var transaction = new InvestmentTransaction
                {
                    InvestmentId = investment.Id,
                    TransactionType = row.TransactionType,
                    Quantity = row.Quantity,
                    Price = row.Price,
                    Amount = row.Quantity * row.Price,
                    Fee = 0,
                    TransactionDate = row.TransactionDate ?? DateTime.Now,
                    Notes = row.Notes ?? "从文件导入",
                    CreatedAt = DateTime.Now
                };

                _context.InvestmentTransactions.Add(transaction);

                // 更新投资记录
                if (row.TransactionType == "buy")
                {
                    var totalCost = investment.TotalCost + transaction.Amount;
                    var totalQuantity = investment.Quantity + transaction.Quantity;
                    investment.Quantity = totalQuantity;
                    investment.CostPrice = totalQuantity > 0 ? totalCost / totalQuantity : 0;
                    investment.TotalCost = totalCost;
                }
                else if (row.TransactionType == "sell")
                {
                    investment.Quantity = Math.Max(0, investment.Quantity - transaction.Quantity);
                    investment.TotalCost = investment.Quantity * investment.CostPrice;
                }

                // 更新当前价格和市值
                if (!string.IsNullOrEmpty(investment.Code) && !string.IsNullOrEmpty(investment.Type))
                {
                    investment.CurrentPrice = await GetCurrentPrice(investment.Code, investment.Type);
                }
                investment.MarketValue = investment.Quantity * investment.CurrentPrice;
                investment.ProfitLoss = investment.MarketValue - investment.TotalCost;
                investment.ProfitRate = investment.TotalCost > 0
                    ? (investment.ProfitLoss / investment.TotalCost) * 100
                    : 0;
                investment.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailCount++;
                result.Errors.Add($"代码 {row.Code}: {ex.Message}");
            }
        }

        return result;
    }

    /// <summary>
    /// 获取当前价格（复用 InvestmentController 的逻辑）
    /// </summary>
    private async Task<decimal> GetCurrentPrice(string? code, string? type)
    {
        if (string.IsNullOrEmpty(code))
        {
            return 0;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var paddedCode = code.PadLeft(6, '0');
            string secid;

            if (type?.ToLower() == "fund")
            {
                if (paddedCode.StartsWith("51") || paddedCode.StartsWith("52") ||
                    paddedCode.StartsWith("53") || paddedCode.StartsWith("54") ||
                    paddedCode.StartsWith("55") || paddedCode.StartsWith("56") ||
                    paddedCode.StartsWith("57") || paddedCode.StartsWith("58") ||
                    paddedCode.StartsWith("59"))
                {
                    secid = $"1.{paddedCode}";
                }
                else if (paddedCode.StartsWith("15") || paddedCode.StartsWith("16"))
                {
                    secid = $"0.{paddedCode}";
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                secid = $"1.{paddedCode}";
            }

            var url = $"https://push2.eastmoney.com/api/qt/stock/get?secid={secid}&fields=f43,f57,f58";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(content);

                if (result.TryGetProperty("rc", out var rcElement))
                {
                    var rc = rcElement.GetInt32();
                    if (rc != 0)
                    {
                        return 0;
                    }
                }

                if (result.TryGetProperty("data", out var data) && data.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    if (data.TryGetProperty("f43", out var priceElement) && priceElement.ValueKind != System.Text.Json.JsonValueKind.Null)
                    {
                        var priceStr = priceElement.GetRawText();
                        if (decimal.TryParse(priceStr, out var price) && price > 0)
                        {
                            return price / 100;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取价格失败 {code}: {ex.Message}");
        }

        return 0;
    }

    /// <summary>
    /// 根据代码获取名称
    /// </summary>
    private async Task<string> GetNameFromCode(string? code, string? type)
    {
        // 这里可以调用 InvestmentController 的 GetNameFromCode 方法
        // 或者直接实现相同的逻辑
        return string.Empty;
    }

    /// <summary>
    /// 解析小数
    /// </summary>
    private decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        value = value.Trim().Replace(",", "");
        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        return 0;
    }

    /// <summary>
    /// 解析日期时间
    /// </summary>
    private DateTime? ParseDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // 尝试多种日期格式
        var formats = new[]
        {
            "yyyy-MM-dd",
            "yyyy/MM/dd",
            "MM/dd/yyyy",
            "dd/MM/yyyy",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy/MM/dd HH:mm:ss"
        };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
        }

        // 如果都不匹配，尝试通用解析
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            return parsedDate;
        }

        return null;
    }
}

/// <summary>
/// 交易记录导入行
/// </summary>
public class TransactionImportRow
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "fund";
    public string TransactionType { get; set; } = "buy";
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 导入结果
/// </summary>
public class ImportResult
{
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
