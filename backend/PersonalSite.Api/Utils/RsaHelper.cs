using System.Security.Cryptography;
using System.Text;

namespace PersonalSite.Api.Utils;

/// <summary>
/// RSA 签名工具类
/// </summary>
public static class RsaHelper
{
    /// <summary>
    /// RSA2 签名
    /// </summary>
    public static string SignRsa2(string data, string privateKey)
    {
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            
            return Convert.ToBase64String(signatureBytes);
        }
        catch (Exception)
        {
            // 如果签名失败，返回空字符串
            return string.Empty;
        }
    }

    /// <summary>
    /// RSA2 验证签名
    /// </summary>
    public static bool VerifyRsa2(string data, string signature, string publicKey)
    {
        try
        {
            using var rsa = RSA.Create();
            
            // 尝试导入 PEM 格式的公钥
            try
            {
                rsa.ImportFromPem(publicKey);
            }
            catch
            {
                // 如果不是 PEM 格式，尝试 XML 格式
                rsa.FromXmlString(publicKey);
            }

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = Convert.FromBase64String(signature);

            return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 构建待签名字符串（支付宝格式）
    /// </summary>
    public static string BuildSignString(Dictionary<string, string> parameters)
    {
        var sortedParams = parameters
            .Where(kv => !string.IsNullOrEmpty(kv.Value) && kv.Key != "sign" && kv.Key != "sign_type")
            .OrderBy(kv => kv.Key)
            .Select(kv => $"{kv.Key}={kv.Value}");

        return string.Join("&", sortedParams);
    }
}

