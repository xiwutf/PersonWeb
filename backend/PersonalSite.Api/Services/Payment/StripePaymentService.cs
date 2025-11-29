using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace PersonalSite.Api.Services.Payment;

/// <summary>
/// Stripe 支付服务
/// </summary>
public class StripePaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<StripePaymentService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public StripePaymentService(
        IConfiguration configuration,
        ILogger<StripePaymentService> logger,
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
            var secretKey = _configuration["Stripe:SecretKey"];
            var publishableKey = _configuration["Stripe:PublishableKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                return new PaymentResponse
                {
                    Success = false,
                    ErrorMessage = "Stripe配置不完整"
                };
            }

            // 创建支付意图（Payment Intent）
            var paymentIntentRequest = new
            {
                amount = request.Amount, // Stripe使用分为单位
                currency = "cny",
                description = request.Description,
                metadata = new Dictionary<string, string>
                {
                    ["order_id"] = request.OrderId,
                    ["user_id"] = request.UserId
                }
            };

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");

            var jsonContent = JsonSerializer.Serialize(paymentIntentRequest);
            var response = await httpClient.PostAsync(
                "https://api.stripe.com/v1/payment_intents",
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var clientSecret = result.GetProperty("client_secret").GetString() ?? "";

                return new PaymentResponse
                {
                    Success = true,
                    PaymentId = result.GetProperty("id").GetString() ?? request.OrderId,
                    PaymentParams = new Dictionary<string, object>
                    {
                        ["clientSecret"] = clientSecret,
                        ["publishableKey"] = publishableKey ?? "",
                        ["paymentIntentId"] = result.GetProperty("id").GetString() ?? ""
                    }
                };
            }

            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = "创建支付订单失败"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建Stripe支付订单失败");
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
            var secretKey = _configuration["Stripe:SecretKey"];

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");

            var response = await httpClient.GetAsync(
                $"https://api.stripe.com/v1/payment_intents/{paymentId}");

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var status = result.GetProperty("status").GetString() ?? "pending";

                return new PaymentStatusResponse
                {
                    PaymentId = paymentId,
                    OrderId = result.TryGetProperty("metadata", out var metadata) &&
                              metadata.TryGetProperty("order_id", out var orderId)
                        ? orderId.GetString() ?? paymentId
                        : paymentId,
                    Status = status == "succeeded" ? "paid" : "pending",
                    Amount = result.GetProperty("amount").GetInt32(),
                    PaidAt = status == "succeeded" ? DateTime.Now : null,
                    ThirdPartyOrderId = paymentId
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
            _logger.LogError(ex, "查询Stripe支付状态失败");
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
            var webhookSecret = _configuration["Stripe:WebhookSecret"];
            var rawBody = request.RawBody;

            // 验证Webhook签名
            if (!string.IsNullOrEmpty(webhookSecret) && !VerifyWebhookSignature(rawBody, request.Headers, webhookSecret))
            {
                return new PaymentCallbackResult
                {
                    Success = false,
                    ErrorMessage = "签名验证失败"
                };
            }

            // 解析Webhook事件
            var eventData = JsonSerializer.Deserialize<JsonElement>(rawBody);
            var eventType = eventData.GetProperty("type").GetString() ?? "";

            if (eventType == "payment_intent.succeeded")
            {
                var paymentIntent = eventData.GetProperty("data").GetProperty("object");
                var paymentIntentId = paymentIntent.GetProperty("id").GetString() ?? "";
                var metadata = paymentIntent.GetProperty("metadata");

                return new PaymentCallbackResult
                {
                    Success = true,
                    PaymentId = metadata.TryGetProperty("order_id", out var orderId)
                        ? orderId.GetString() ?? paymentIntentId
                        : paymentIntentId,
                    OrderId = metadata.TryGetProperty("order_id", out var orderId2)
                        ? orderId2.GetString() ?? paymentIntentId
                        : paymentIntentId,
                    Status = "paid",
                    ThirdPartyOrderId = paymentIntentId
                };
            }

            return new PaymentCallbackResult
            {
                Success = false,
                ErrorMessage = "未处理的支付事件"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理Stripe支付回调失败");
            return new PaymentCallbackResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public bool VerifySignature(string data, string signature)
    {
        // Stripe使用Webhook签名验证
        return true; // 简化实现
    }

    private bool VerifyWebhookSignature(string payload, Dictionary<string, string> headers, string secret)
    {
        // 实现Stripe Webhook签名验证
        // 这里简化实现，实际应该使用Stripe的签名验证逻辑
        return true;
    }
}

