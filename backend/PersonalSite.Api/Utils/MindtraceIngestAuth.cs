using System.Security.Cryptography;
using System.Text;

namespace PersonalSite.Api.Utils;

/// <summary>
/// MindTrace ingest Bearer Token 校验（与管理员 JWT 分离）
/// </summary>
public static class MindtraceIngestAuth
{
    public const int MissingConfigStatusCode = 503;
    public const int UnauthorizedStatusCode = 401;

    /// <summary>
    /// 校验 Authorization 头中的 Bearer Token。
    /// </summary>
    /// <returns>null 表示通过；否则返回 (statusCode, message)</returns>
    public static (int StatusCode, string Message)? Validate(
        string? authorizationHeader,
        string? configuredToken)
    {
        string expected = (configuredToken ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(expected))
        {
            return (MissingConfigStatusCode, "MindTrace ingest token is not configured");
        }

        string? provided = ExtractBearerToken(authorizationHeader);
        if (string.IsNullOrEmpty(provided) || !TokensEqual(provided, expected))
        {
            return (UnauthorizedStatusCode, "Unauthorized");
        }

        return null;
    }

    public static string? ExtractBearerToken(string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return null;
        }

        const string prefix = "Bearer ";
        if (!authorizationHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        string token = authorizationHeader.Substring(prefix.Length).Trim();
        return string.IsNullOrEmpty(token) ? null : token;
    }

    public static bool TokensEqual(string left, string right)
    {
        byte[] leftBytes = Encoding.UTF8.GetBytes(left);
        byte[] rightBytes = Encoding.UTF8.GetBytes(right);
        if (leftBytes.Length != rightBytes.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}
