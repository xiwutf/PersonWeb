using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using PersonalSite.Api.Services.Payment;
using PersonalSite.Api.Utils;
using System.Text;

namespace PersonalSite.Api.Controllers;

/// <summary>
/// 支付控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly PaymentServiceFactory _paymentServiceFactory;
    private readonly ILogger<PaymentController> _logger;
    private readonly AppDbContext _context;

    public PaymentController(
        PaymentServiceFactory paymentServiceFactory,
        ILogger<PaymentController> logger,
        AppDbContext context)
    {
        _paymentServiceFactory = paymentServiceFactory;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// 创建支付订单
    /// </summary>
    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<object>>> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            var paymentService = _paymentServiceFactory.GetPaymentService(request.PaymentMethod);

            var paymentRequest = new PaymentRequest
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Description = request.Description,
                UserId = request.UserId,
                PaymentMethod = request.PaymentMethod,
                ClientIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                NotifyUrl = request.NotifyUrl,
                ReturnUrl = request.ReturnUrl,
                ExtraData = request.ExtraData
            };

            var response = await paymentService.CreatePaymentAsync(paymentRequest);

            if (!response.Success)
            {
                return BadRequest(ApiResponse.Error(response.ErrorMessage ?? "创建支付订单失败", 400));
            }

            return Ok(ApiResponse.Success(new
            {
                PaymentId = response.PaymentId,
                PaymentUrl = response.PaymentUrl,
                QrCode = response.QrCode,
                PaymentParams = response.PaymentParams
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建支付订单失败");
            return StatusCode(500, ApiResponse.Error($"创建支付订单失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 查询支付状态
    /// </summary>
    [HttpGet("status/{paymentId}")]
    public async Task<ActionResult<ApiResponse<object>>> GetPaymentStatus(
        string paymentId,
        [FromQuery] string paymentMethod = "wechat")
    {
        try
        {
            var paymentService = _paymentServiceFactory.GetPaymentService(paymentMethod);
            var status = await paymentService.QueryPaymentStatusAsync(paymentId);

            return Ok(ApiResponse.Success(status));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询支付状态失败");
            return StatusCode(500, ApiResponse.Error($"查询支付状态失败: {ex.Message}", 500));
        }
    }

    /// <summary>
    /// 微信支付回调
    /// </summary>
    [HttpPost("callback/wechat")]
    public async Task<IActionResult> WeChatCallback()
    {
        try
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var rawBody = await reader.ReadToEndAsync();

            var data = ParseXmlToDictionary(rawBody);

            var callbackRequest = new PaymentCallbackRequest
            {
                PaymentMethod = "wechat",
                Data = data,
                RawBody = rawBody,
                Headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
            };

            var paymentService = _paymentServiceFactory.GetPaymentService("wechat");
            var result = await paymentService.HandleCallbackAsync(callbackRequest);

            // 处理支付成功逻辑（更新订单状态等）
            if (result.Success)
            {
                await HandlePaymentSuccessAsync(result);
            }

            return Content(result.ResponseContent ?? "<xml><return_code><![CDATA[SUCCESS]]></return_code></xml>", "application/xml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理微信支付回调失败");
            return Content("<xml><return_code><![CDATA[FAIL]]></return_code></xml>", "application/xml");
        }
    }

    /// <summary>
    /// 支付宝回调
    /// </summary>
    [HttpPost("callback/alipay")]
    public async Task<IActionResult> AlipayCallback()
    {
        try
        {
            var data = Request.Form.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

            var callbackRequest = new PaymentCallbackRequest
            {
                PaymentMethod = "alipay",
                Data = data,
                RawBody = "",
                Headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
            };

            var paymentService = _paymentServiceFactory.GetPaymentService("alipay");
            var result = await paymentService.HandleCallbackAsync(callbackRequest);

            if (result.Success)
            {
                await HandlePaymentSuccessAsync(result);
            }

            return Content(result.ResponseContent ?? "success", "text/plain");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理支付宝回调失败");
            return Content("fail", "text/plain");
        }
    }

    /// <summary>
    /// Stripe Webhook
    /// </summary>
    [HttpPost("callback/stripe")]
    public async Task<IActionResult> StripeWebhook()
    {
        try
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var rawBody = await reader.ReadToEndAsync();

            var callbackRequest = new PaymentCallbackRequest
            {
                PaymentMethod = "stripe",
                Data = new Dictionary<string, string>(),
                RawBody = rawBody,
                Headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
            };

            var paymentService = _paymentServiceFactory.GetPaymentService("stripe");
            var result = await paymentService.HandleCallbackAsync(callbackRequest);

            if (result.Success)
            {
                await HandlePaymentSuccessAsync(result);
            }

            return Ok(new { received = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理Stripe Webhook失败");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// 处理支付成功
    /// </summary>
    private async Task HandlePaymentSuccessAsync(PaymentCallbackResult result)
    {
        try
        {
            _logger.LogInformation(
                "支付成功: OrderId={OrderId}, PaymentId={PaymentId}, Status={Status}",
                result.OrderId, result.PaymentId, result.Status);

            // 根据 OrderId 查找对应的订单
            // OrderId 格式可能是: purchase_id 或 membership_id 等

            // 1. 查找主题购买订单
            if (long.TryParse(result.OrderId, out long purchaseId))
            {
                var themePurchase = await _context.ThemePurchases.FindAsync(purchaseId);
                if (themePurchase != null && themePurchase.PaymentStatus == "pending")
                {
                    themePurchase.PaymentStatus = "paid";
                    themePurchase.IsActive = true;
                    themePurchase.PaidAt = DateTime.Now;
                    themePurchase.PaymentId = result.ThirdPartyOrderId ?? result.PaymentId;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("主题购买订单已更新: PurchaseId={PurchaseId}", purchaseId);
                    return;
                }

                // 2. 查找工具购买订单
                var toolPurchase = await _context.ToolPurchases.FindAsync(purchaseId);
                if (toolPurchase != null && toolPurchase.PaymentStatus == "pending")
                {
                    toolPurchase.PaymentStatus = "paid";
                    toolPurchase.IsActive = true;
                    toolPurchase.PaidAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("工具购买订单已更新: PurchaseId={PurchaseId}", purchaseId);
                    return;
                }

                // 3. 查找会员订单
                var membership = await _context.UserMemberships.FindAsync(purchaseId);
                if (membership != null && membership.Status != "active")
                {
                    membership.Status = "active";
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("会员订单已更新: MembershipId={MembershipId}", purchaseId);
                    return;
                }
            }

            _logger.LogWarning("未找到对应的订单: OrderId={OrderId}", result.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理支付成功时发生错误: OrderId={OrderId}", result.OrderId);
        }
    }

    /// <summary>
    /// 解析XML到字典
    /// </summary>
    private Dictionary<string, string> ParseXmlToDictionary(string xml)
    {
        return XmlHelper.ParseXmlToDictionary(xml);
    }
}

/// <summary>
/// 创建支付请求
/// </summary>
public class CreatePaymentRequest
{
    public string OrderId { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "wechat";
    public string? NotifyUrl { get; set; }
    public string? ReturnUrl { get; set; }
    public Dictionary<string, string>? ExtraData { get; set; }
}

