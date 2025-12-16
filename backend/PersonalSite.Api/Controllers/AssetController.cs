using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;

namespace PersonalSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetController : ControllerBase
{
    private readonly AppDbContext _context;

    public AssetController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取资产列表
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetList()
    {
        var assets = await _context.Assets
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.UpdatedAt)
            .Select(a => new
            {
                a.Id,
                a.Name,
                a.AssetType,
                a.Institution,
                a.Amount,
                a.Currency,
                a.InterestRate,
                a.MaturityDate,
                a.Notes,
                a.IsActive,
                a.CreatedAt,
                a.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse.Success(assets));
    }

    /// <summary>
    /// 获取资产详情
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetById(long id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null)
        {
            return Ok(ApiResponse.Error("未找到", 404));
        }

        return Ok(ApiResponse.Success(asset));
    }

    /// <summary>
    /// 创建资产记录
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Create([FromBody] AssetRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return Ok(ApiResponse.Error("资产名称不能为空", 400));
            }

            var asset = new Asset
            {
                Name = request.Name,
                AssetType = request.AssetType ?? "bank_card",
                Institution = request.Institution,
                Amount = request.Amount,
                Currency = request.Currency ?? "CNY",
                InterestRate = request.InterestRate ?? 0,
                MaturityDate = request.MaturityDate,
                Notes = request.Notes,
                IsActive = request.IsActive ?? true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(new { id = asset.Id }, "创建成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"创建失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 更新资产记录
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Update(long id, [FromBody] AssetRequest request)
    {
        try
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return Ok(ApiResponse.Error("未找到", 404));
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                asset.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.AssetType))
            {
                asset.AssetType = request.AssetType;
            }

            asset.Institution = request.Institution ?? asset.Institution;
            asset.Amount = request.Amount;
            asset.Currency = request.Currency ?? asset.Currency;
            asset.InterestRate = request.InterestRate ?? asset.InterestRate;
            asset.MaturityDate = request.MaturityDate ?? asset.MaturityDate;
            asset.Notes = request.Notes ?? asset.Notes;

            if (request.IsActive.HasValue)
            {
                asset.IsActive = request.IsActive.Value;
            }

            asset.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "更新成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"更新失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 删除资产记录
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Delete(long id)
    {
        try
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return Ok(ApiResponse.Error("未找到", 404));
            }

            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Success(null, "删除成功"));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"删除失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取资产总览统计
    /// </summary>
    [HttpGet("overview")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetOverview()
    {
        try
        {
            // 获取所有资产
            var assets = await _context.Assets
                .Where(a => a.IsActive)
                .ToListAsync();

            // 获取所有投资
            var investments = await _context.Investments.ToListAsync();

            // 计算总资产
            var totalAssets = assets.Sum(a => a.Amount);
            
            // 计算投资数据
            var totalCost = investments.Sum(i => i.TotalCost);
            // 计算实际市值：如果 MarketValue > 0 使用 MarketValue，否则使用 TotalCost（至少显示投资成本）
            var actualMarketValue = investments.Sum(i => i.MarketValue > 0 ? i.MarketValue : i.TotalCost);
            // 计算盈亏：如果 MarketValue > 0 使用实际盈亏，否则为 0（价格未知时无法计算盈亏）
            var totalProfitLoss = investments.Sum(i => i.MarketValue > 0 ? i.ProfitLoss : 0);
            
            // 总投资使用实际市值（包含备用值）
            var totalInvestments = actualMarketValue;
            var totalNetWorth = totalAssets + totalInvestments;
            
            // 只在有错误时记录日志

            // 按资产类型统计
            var assetsByType = assets
                .GroupBy(a => a.AssetType)
                .Select(g => new
                {
                    Type = g.Key,
                    TypeName = GetAssetTypeName(g.Key),
                    Count = g.Count(),
                    TotalAmount = g.Sum(a => a.Amount),
                    Percentage = totalAssets > 0 ? (g.Sum(a => a.Amount) / totalAssets) * 100 : 0
                })
                .ToList();

            // 投资统计（使用上面计算的值）
            var investmentStats = new
            {
                TotalCost = totalCost,
                TotalMarketValue = actualMarketValue,
                TotalProfitLoss = totalProfitLoss,
                TotalProfitRate = totalCost > 0
                    ? (totalProfitLoss / totalCost) * 100
                    : 0,
                Count = investments.Count
            };

            return Ok(ApiResponse.Success(new
            {
                TotalAssets = totalAssets,
                TotalInvestments = totalInvestments,
                TotalNetWorth = totalNetWorth,
                AssetsByType = assetsByType,
                InvestmentStats = investmentStats,
                AssetDistribution = new
                {
                    Assets = totalAssets,
                    Investments = actualMarketValue, // 使用实际市值（包含备用值）
                    AssetsPercentage = totalNetWorth > 0 ? (totalAssets / totalNetWorth) * 100 : 0,
                    InvestmentsPercentage = totalNetWorth > 0 ? (actualMarketValue / totalNetWorth) * 100 : 0
                }
            }));
        }
        catch (Exception ex)
        {
            return Ok(ApiResponse.Error($"获取总览失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 获取资产类型名称
    /// </summary>
    private string GetAssetTypeName(string type)
    {
        return type?.ToLower() switch
        {
            "bank_card" => "银行卡",
            "deposit" => "存单",
            "cash" => "现金",
            "other" => "其他",
            _ => type ?? "未知"
        };
    }
}

/// <summary>
/// 资产请求模型
/// </summary>
public class AssetRequest
{
    public string? Name { get; set; }
    public string? AssetType { get; set; }
    public string? Institution { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public decimal? InterestRate { get; set; }
    public DateTime? MaturityDate { get; set; }
    public string? Notes { get; set; }
    public bool? IsActive { get; set; }
}
