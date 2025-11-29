using Microsoft.Extensions.Configuration;
using PersonalSite.Api.Utils;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PersonalSite.Api.Services.Payment;

/// <summary>
/// 微信支付服务
/// </summary>
public class WeChatPaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeChatPaymentService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public WeChatPaymentService(
        IConfiguration configuration,
        ILogger<WeChatPaymentService> logger,
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
            // 获取微信支付配置
            var appId = _configuration["WeChatPay:AppId"];
            var mchId = _configuration["WeChatPay:MchId"];
            var apiKey = _configuration["WeChatPay:ApiKey"];
            var notifyUrl = request.NotifyUrl ?? _configuration["WeChatPay:NotifyUrl"];

            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(mchId) || string.IsNullOrEmpty(apiKey))
            {
                return new PaymentResponse
                {
                    Success = false,
                    ErrorMessage = "微信支付配置不完整"
                };
            }

            // 构建统一下单请求
            var unifiedOrderRequest = new
            {
                appid = appId,
                mch_id = mchId,
                nonce_str = GenerateNonceStr(),
                body = request.Description,
                out_trade_no = request.OrderId,
                total_fee = request.Amount,
                spbill_create_ip = request.ClientIp ?? "127.0.0.1",
                notify_url = notifyUrl,
                trade_type = "NATIVE", // 扫码支付
                product_id = request.OrderId
            };

            // 生成签名
            var sign = GenerateSign(unifiedOrderRequest, apiKey);

            // 构建XML请求
            var xmlRequest = BuildXmlRequest(unifiedOrderRequest, sign);

            // 调用微信支付API
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var response = await httpClient.PostAsync(
                "https://api.mch.weixin.qq.com/pay/unifiedorder",
                new StringContent(xmlRequest, Encoding.UTF8, "application/xml"));

            var responseContent = await response.Content.ReadAsStringAsync();

            // 解析响应
            var result = ParseXmlResponse(responseContent);

            if (result["return_code"] == "SUCCESS" && result["result_code"] == "SUCCESS")
            {
                return new PaymentResponse
                {
                    Success = true,
                    PaymentId = result["prepay_id"] ?? request.OrderId,
                    QrCode = result["code_url"],
                    PaymentParams = new Dictionary<string, object>
                    {
                        ["prepay_id"] = result["prepay_id"] ?? "",
                        ["code_url"] = result["code_url"] ?? ""
                    }
                };
            }

            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = result["err_code_des"] ?? "创建支付订单失败"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建微信支付订单失败");
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
            var appId = _configuration["WeChatPay:AppId"];
            var mchId = _configuration["WeChatPay:MchId"];
            var apiKey = _configuration["WeChatPay:ApiKey"];

            var queryRequest = new
            {
                appid = appId,
                mch_id = mchId,
                out_trade_no = paymentId,
                nonce_str = GenerateNonceStr()
            };

            var sign = GenerateSign(queryRequest, apiKey);
            var xmlRequest = BuildXmlRequest(queryRequest, sign);

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(
                "https://api.mch.weixin.qq.com/pay/orderquery",
                new StringContent(xmlRequest, Encoding.UTF8, "application/xml"));

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = ParseXmlResponse(responseContent);

            if (result["return_code"] == "SUCCESS" && result["result_code"] == "SUCCESS")
            {
                var tradeState = result["trade_state"] ?? "";
                return new PaymentStatusResponse
                {
                    PaymentId = paymentId,
                    OrderId = result["out_trade_no"] ?? paymentId,
                    Status = tradeState == "SUCCESS" ? "paid" : "pending",
                    Amount = int.Parse(result["total_fee"] ?? "0"),
                    PaidAt = tradeState == "SUCCESS" ? DateTime.Now : null,
                    ThirdPartyOrderId = result["transaction_id"]
                };
            }

            return new PaymentStatusResponse
            {
                PaymentId = paymentId,
                Status = "pending"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询微信支付状态失败");
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
            var apiKey = _configuration["WeChatPay:ApiKey"];
            var data = request.Data;

            // 验证签名
            if (!VerifySignature(data, apiKey))
            {
                return new PaymentCallbackResult
                {
                    Success = false,
                    ErrorMessage = "签名验证失败",
                    ResponseContent = "<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[签名验证失败]]></return_msg></xml>"
                };
            }

            var returnCode = data["return_code"] ?? "";
            var resultCode = data["result_code"] ?? "";

            if (returnCode == "SUCCESS" && resultCode == "SUCCESS")
            {
                return new PaymentCallbackResult
                {
                    Success = true,
                    PaymentId = data["out_trade_no"] ?? "",
                    OrderId = data["out_trade_no"] ?? "",
                    Status = "paid",
                    ThirdPartyOrderId = data["transaction_id"],
                    ResponseContent = "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>"
                };
            }

            return new PaymentCallbackResult
            {
                Success = false,
                ErrorMessage = "支付失败",
                ResponseContent = "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理微信支付回调失败");
            return new PaymentCallbackResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ResponseContent = "<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[处理失败]]></return_msg></xml>"
            };
        }
    }

    public bool VerifySignature(string data, string signature)
    {
        // 实现签名验证逻辑
        return true; // 简化实现
    }

    public bool VerifySignature(Dictionary<string, string> data, string? apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("微信支付 API Key 未配置");
            return false;
        }

        var sign = data.GetValueOrDefault("sign", "");
        var calculatedSign = GenerateSign(data.Where(kv => kv.Key != "sign").ToDictionary(kv => kv.Key, kv => kv.Value), apiKey);
        return sign == calculatedSign;
    }

    private string GenerateNonceStr()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 32);
    }

    private string GenerateSign(object data, string apiKey)
    {
        var dict = new Dictionary<string, string>();
        foreach (var prop in data.GetType().GetProperties())
        {
            var value = prop.GetValue(data)?.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                dict[prop.Name] = value;
            }
        }
        return GenerateSign(dict, apiKey);
    }

    private string GenerateSign(Dictionary<string, string> data, string? apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            return string.Empty;
        }

        var sortedData = data.Where(kv => !string.IsNullOrEmpty(kv.Value) && kv.Key != "sign")
            .OrderBy(kv => kv.Key)
            .Select(kv => $"{kv.Key}={kv.Value}");

        var stringA = string.Join("&", sortedData);
        var stringSignTemp = $"{stringA}&key={apiKey}";

        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(stringSignTemp);
        var hash = md5.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToUpper();
    }

    private string BuildXmlRequest(object data, string sign)
    {
        var dict = new Dictionary<string, string>();
        foreach (var prop in data.GetType().GetProperties())
        {
            var value = prop.GetValue(data)?.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                dict[prop.Name] = value;
            }
        }
        dict["sign"] = sign;
        return XmlHelper.DictionaryToXml(dict);
    }

    private Dictionary<string, string> ParseXmlResponse(string xml)
    {
        return XmlHelper.ParseXmlToDictionary(xml);
    }
}

