namespace PersonalSite.Api.Models.Dto;

/// <summary>
/// 通用请求模型
/// </summary>

/// <summary>
/// 字符串类型的用户ID请求（用于访客系统、主题商店等）
/// </summary>
public class UserIdRequest
{
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// 长整型用户ID请求（用于API用户系统）
/// </summary>
public class ApiUserIdRequest
{
    public long UserId { get; set; }
}

/// <summary>
/// 访客ID请求
/// </summary>
public class VisitorIdRequest
{
    public string VisitorId { get; set; } = string.Empty;
}

