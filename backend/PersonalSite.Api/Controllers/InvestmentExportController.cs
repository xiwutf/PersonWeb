using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using System.Text;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestmentExportController : ControllerBase
{
    private readonly AppDbContext _context;

    public InvestmentExportController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 导出投资记录为 CSV
    /// </summary>
    [HttpGet("investments/csv")]
    [Authorize]
    public async Task<IActionResult> ExportInvestmentsCsv()
    {
        try
        {
            var investments = await _context.Investments
                .OrderByDescending(i => i.UpdatedAt)
                .ToListAsync();

            var csv = new StringBuilder();
            
            // CSV 表头
            csv.AppendLine("代码,名称,类型,持仓数量,成本价,当前价,总成本,市值,盈亏,收益率(%),备注,创建时间,更新时间");

            // CSV 数据行
            foreach (var investment in investments)
            {
                csv.AppendLine($"{EscapeCsvField(investment.Code)}," +
                              $"{EscapeCsvField(investment.Name)}," +
                              $"{EscapeCsvField(investment.Type)}," +
                              $"{investment.Quantity}," +
                              $"{investment.CostPrice}," +
                              $"{investment.CurrentPrice}," +
                              $"{investment.TotalCost}," +
                              $"{investment.MarketValue}," +
                              $"{investment.ProfitLoss}," +
                              $"{investment.ProfitRate:F2}," +
                              $"{EscapeCsvField(investment.Notes ?? "")}," +
                              $"{investment.CreatedAt:yyyy-MM-dd HH:mm:ss}," +
                              $"{investment.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"投资记录_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            return File(bytes, "text/csv; charset=utf-8", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"导出失败: {ex.Message}" });
        }
    }

    /// <summary>
    /// 导出交易记录为 CSV
    /// </summary>
    [HttpGet("transactions/csv")]
    [Authorize]
    public async Task<IActionResult> ExportTransactionsCsv()
    {
        try
        {
            var transactions = await _context.InvestmentTransactions
                .Include(t => t.Investment)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            var csv = new StringBuilder();
            
            // CSV 表头
            csv.AppendLine("投资代码,投资名称,交易类型,数量,价格,金额,手续费,交易日期,备注,创建时间");

            // CSV 数据行
            foreach (var transaction in transactions)
            {
                csv.AppendLine($"{EscapeCsvField(transaction.Investment?.Code ?? "")}," +
                              $"{EscapeCsvField(transaction.Investment?.Name ?? "")}," +
                              $"{EscapeCsvField(transaction.TransactionType)}," +
                              $"{transaction.Quantity}," +
                              $"{transaction.Price}," +
                              $"{transaction.Amount}," +
                              $"{transaction.Fee}," +
                              $"{transaction.TransactionDate:yyyy-MM-dd HH:mm:ss}," +
                              $"{EscapeCsvField(transaction.Notes ?? "")}," +
                              $"{transaction.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"交易记录_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            return File(bytes, "text/csv; charset=utf-8", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"导出失败: {ex.Message}" });
        }
    }

    /// <summary>
    /// 导出统计数据为 CSV
    /// </summary>
    [HttpGet("stats/csv")]
    [Authorize]
    public async Task<IActionResult> ExportStatsCsv()
    {
        try
        {
            var investments = await _context.Investments.ToListAsync();

            var totalCost = investments.Sum(i => i.TotalCost);
            var totalMarketValue = investments.Sum(i => i.MarketValue);
            var totalProfitLoss = totalMarketValue - totalCost;
            var totalProfitRate = totalCost > 0 ? (totalProfitLoss / totalCost) * 100 : 0;

            var csv = new StringBuilder();
            
            // CSV 表头
            csv.AppendLine("统计项,数值");
            csv.AppendLine($"总成本,{totalCost}");
            csv.AppendLine($"总市值,{totalMarketValue}");
            csv.AppendLine($"总盈亏,{totalProfitLoss}");
            csv.AppendLine($"总收益率(%),{totalProfitRate:F2}");
            csv.AppendLine($"持仓数量,{investments.Count}");

            // 按类型统计
            csv.AppendLine("");
            csv.AppendLine("按类型统计");
            csv.AppendLine("类型,数量,总成本,总市值,盈亏,收益率(%)");
            
            var byType = investments
                .GroupBy(i => i.Type)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count(),
                    TotalCost = g.Sum(i => i.TotalCost),
                    TotalMarketValue = g.Sum(i => i.MarketValue),
                    ProfitLoss = g.Sum(i => i.ProfitLoss),
                    ProfitRate = g.Sum(i => i.TotalCost) > 0 
                        ? (g.Sum(i => i.ProfitLoss) / g.Sum(i => i.TotalCost)) * 100 
                        : 0
                })
                .ToList();

            foreach (var item in byType)
            {
                csv.AppendLine($"{item.Type},{item.Count},{item.TotalCost},{item.TotalMarketValue},{item.ProfitLoss},{item.ProfitRate:F2}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"统计数据_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            return File(bytes, "text/csv; charset=utf-8", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"导出失败: {ex.Message}" });
        }
    }

    /// <summary>
    /// 转义 CSV 字段（处理包含逗号、引号、换行符的字段）
    /// </summary>
    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return "";

        // 如果字段包含逗号、引号或换行符，需要用引号包裹，并转义内部引号
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
        {
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }

        return field;
    }
}
