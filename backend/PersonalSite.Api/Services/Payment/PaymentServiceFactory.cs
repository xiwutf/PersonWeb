using Microsoft.Extensions.Configuration;

namespace PersonalSite.Api.Services.Payment;

/// <summary>
/// 支付服务工厂
/// </summary>
public class PaymentServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public PaymentServiceFactory(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    /// <summary>
    /// 获取支付服务实例
    /// </summary>
    /// <param name="paymentMethod">支付方式：wechat, alipay, stripe</param>
    /// <returns>支付服务实例</returns>
    public IPaymentService GetPaymentService(string paymentMethod)
    {
        return paymentMethod.ToLower() switch
        {
            "wechat" or "wechatpay" => _serviceProvider.GetRequiredService<WeChatPaymentService>(),
            "alipay" => _serviceProvider.GetRequiredService<AlipayPaymentService>(),
            "stripe" => _serviceProvider.GetRequiredService<StripePaymentService>(),
            _ => throw new NotSupportedException($"不支持的支付方式: {paymentMethod}")
        };
    }
}

