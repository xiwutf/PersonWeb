namespace PersonalSite.Api.Services.Payment;

/// <summary>
/// 支付服务接口
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// 创建支付订单
    /// </summary>
    /// <param name="request">支付请求</param>
    /// <returns>支付响应</returns>
    Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request);

    /// <summary>
    /// 查询支付状态
    /// </summary>
    /// <param name="paymentId">支付订单号</param>
    /// <returns>支付状态</returns>
    Task<PaymentStatusResponse> QueryPaymentStatusAsync(string paymentId);

    /// <summary>
    /// 处理支付回调
    /// </summary>
    /// <param name="request">回调请求</param>
    /// <returns>处理结果</returns>
    Task<PaymentCallbackResult> HandleCallbackAsync(PaymentCallbackRequest request);

    /// <summary>
    /// 验证支付签名
    /// </summary>
    /// <param name="data">回调数据</param>
    /// <param name="signature">签名</param>
    /// <returns>是否有效</returns>
    bool VerifySignature(string data, string signature);
}

/// <summary>
/// 支付请求
/// </summary>
public class PaymentRequest
{
    /// <summary>
    /// 订单号
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 订单金额（分）
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// 订单描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 支付方式：wechat, alipay, stripe
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// 客户端IP
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// 回调URL
    /// </summary>
    public string? NotifyUrl { get; set; }

    /// <summary>
    /// 返回URL
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, string>? ExtraData { get; set; }
}

/// <summary>
/// 支付响应
/// </summary>
public class PaymentResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 支付订单号
    /// </summary>
    public string PaymentId { get; set; } = string.Empty;

    /// <summary>
    /// 支付URL（用于跳转支付）
    /// </summary>
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// 支付二维码（用于扫码支付）
    /// </summary>
    public string? QrCode { get; set; }

    /// <summary>
    /// 支付参数（用于前端调用支付SDK）
    /// </summary>
    public Dictionary<string, object>? PaymentParams { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 支付状态响应
/// </summary>
public class PaymentStatusResponse
{
    /// <summary>
    /// 支付订单号
    /// </summary>
    public string PaymentId { get; set; } = string.Empty;

    /// <summary>
    /// 订单号
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 支付状态：pending, paid, failed, cancelled
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// 支付金额（分）
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// 支付时间
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// 第三方支付订单号
    /// </summary>
    public string? ThirdPartyOrderId { get; set; }
}

/// <summary>
/// 支付回调请求
/// </summary>
public class PaymentCallbackRequest
{
    /// <summary>
    /// 支付方式
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// 回调数据
    /// </summary>
    public Dictionary<string, string> Data { get; set; } = new();

    /// <summary>
    /// 原始请求体
    /// </summary>
    public string RawBody { get; set; } = string.Empty;

    /// <summary>
    /// 请求头
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();
}

/// <summary>
/// 支付回调结果
/// </summary>
public class PaymentCallbackResult
{
    /// <summary>
    /// 是否成功处理
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 支付订单号
    /// </summary>
    public string PaymentId { get; set; } = string.Empty;

    /// <summary>
    /// 订单号
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 支付状态
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// 第三方支付订单号
    /// </summary>
    public string? ThirdPartyOrderId { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 响应内容（返回给支付平台）
    /// </summary>
    public string? ResponseContent { get; set; }
}

