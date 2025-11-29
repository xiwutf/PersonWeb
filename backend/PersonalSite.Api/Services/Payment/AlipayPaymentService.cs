using Microsoft.Extensions.Configuration;
using PersonalSite.Api.Utils;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PersonalSite.Api.Services.Payment;

/// <summary>
/// 支付宝支付服务
/// </summary>
public class AlipayPaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AlipayPaymentService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public AlipayPaymentService(
        IConfiguration configuration,
        ILogger<AlipayPaymentService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
    {
        try
        {
            var appId = _configuration["Alipay:AppId"];
            var privateKey = _configuration["Alipay:PrivateKey"];
            var publicKey = _configuration["Alipay:PublicKey"];
            var notifyUrl = request.NotifyUrl ?? _configuration["Alipay:NotifyUrl"];
            var returnUrl = request.ReturnUrl ?? _configuration["Alipay:ReturnUrl"];

            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(privateKey))
            {
                return new PaymentResponse
                {
                    Success = false,
                    ErrorMessage = "支付宝配置不完整"
                };
            }

            // 构建支付参数
            var bizContent = new
            {
                out_trade_no = request.OrderId,
                total_amount = (request.Amount / 100.0).ToString("F2"),
                subject = request.Description,
                product_code = "QUICK_MSECURITY_PAY"
            };

            var bizContentJson = JsonSerializer.Serialize(bizContent);

            // 构建请求参数
            var parameters = new Dictionary<string, string>
            {
                ["app_id"] = appId,
                ["method"] = "alipay.trade.app.pay",
                ["charset"] = "utf-8",
                ["sign_type"] = "RSA2",
                ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["version"] = "1.0",
                ["notify_url"] = notifyUrl ?? "",
                ["return_url"] = returnUrl ?? "",
                ["biz_content"] = bizContentJson
            };

            // 生成签名
            var sign = GenerateSign(parameters, privateKey);
            parameters["sign"] = sign;

            // 构建支付URL
            var paymentUrl = BuildPaymentUrl(parameters);

            return new PaymentResponse
            {
                Success = true,
                PaymentId = request.OrderId,
                PaymentUrl = paymentUrl,
                PaymentParams = parameters.ToDictionary(kv => kv.Key, kv => (object)kv.Value)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建支付宝支付订单失败");
            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = $"创建支付订单失败: {ex.Message}"
            };
        }
    }

    public async Task<PaymentStatusResponse> QueryPaymentStatusAsync(string paymentId)
    {
        try
        {
            var appId = _configuration["Alipay:AppId"];
            var privateKey = _configuration["Alipay:PrivateKey"];

            var parameters = new Dictionary<string, string>
            {
                ["app_id"] = appId ?? "",
                ["method"] = "alipay.trade.query",
                ["charset"] = "utf-8",
                ["sign_type"] = "RSA2",
                ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["version"] = "1.0",
                ["biz_content"] = JsonSerializer.Serialize(new { out_trade_no = paymentId })
            };

            var sign = GenerateSign(parameters, privateKey ?? "");
            parameters["sign"] = sign;

            // 调用支付宝查询接口
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(
                "https://openapi.alipay.com/gateway.do",
                new FormUrlEncodedContent(parameters));

            var responseContent = await response.Content.ReadAsStringAsync();
            // 解析响应（简化实现）
            
            return new PaymentStatusResponse
            {
                PaymentId = paymentId,
                Status = "pending"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询支付宝支付状态失败");
            return new PaymentStatusResponse
            {
                PaymentId = paymentId,
                Status = "pending"
            };
        }
    }

    public async Task<PaymentCallbackResult> HandleCallbackAsync(PaymentCallbackRequest request)
    {
        try
        {
            var publicKey = _configuration["Alipay:PublicKey"];
            var data = request.Data;

            // 验证签名
            if (!VerifySignature(data, publicKey))
            {
                return new PaymentCallbackResult
                {
                    Success = false,
                    ErrorMessage = "签名验证失败",
                    ResponseContent = "fail"
                };
            }

            var tradeStatus = data["trade_status"] ?? "";

            if (tradeStatus == "TRADE_SUCCESS" || tradeStatus == "TRADE_FINISHED")
            {
                return new PaymentCallbackResult
                {
                    Success = true,
                    PaymentId = data["out_trade_no"] ?? "",
                    OrderId = data["out_trade_no"] ?? "",
                    Status = "paid",
                    ThirdPartyOrderId = data["trade_no"],
                    ResponseContent = "success"
                };
            }

            return new PaymentCallbackResult
            {
                Success = false,
                ErrorMessage = "支付未完成",
                ResponseContent = "success"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理支付宝支付回调失败");
            return new PaymentCallbackResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ResponseContent = "fail"
            };
        }
    }

    public bool VerifySignature(string data, string signature)
    {
        // 实现签名验证
        return true; // 简化实现
    }

    private string GenerateSign(Dictionary<string, string> parameters, string privateKey)
    {
        if (string.IsNullOrEmpty(privateKey))
        {
            _logger.LogWarning("支付宝私钥未配置");
            return string.Empty;
        }

        try
        {
            var signString = RsaHelper.BuildSignString(parameters);
            return RsaHelper.SignRsa2(signString, privateKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成支付宝签名失败");
            return string.Empty;
        }
    }

    private bool VerifySignature(Dictionary<string, string> data, string? publicKey)
    {
        if (string.IsNullOrEmpty(publicKey))
        {
            _logger.LogWarning("支付宝公钥未配置");
            return false;
        }

        try
        {
            var sign = data.GetValueOrDefault("sign", "");
            if (string.IsNullOrEmpty(sign))
            {
                return false;
            }

            var signString = RsaHelper.BuildSignString(data);
            return RsaHelper.VerifyRsa2(signString, sign, publicKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证支付宝签名失败");
            return false;
        }
    }

    private string BuildPaymentUrl(Dictionary<string, string> parameters)
    {
        var queryString = string.Join("&", parameters.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
        return $"https://openapi.alipay.com/gateway.do?{queryString}";
    }
}

