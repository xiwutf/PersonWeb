using System.Xml.Linq;

namespace PersonalSite.Api.Utils;

/// <summary>
/// XML 辅助工具类
/// </summary>
public static class XmlHelper
{
    /// <summary>
    /// 解析 XML 字符串为字典
    /// </summary>
    public static Dictionary<string, string> ParseXmlToDictionary(string xml)
    {
        var result = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(xml))
        {
            return result;
        }

        try
        {
            var doc = XDocument.Parse(xml);
            var root = doc.Root;

            if (root != null)
            {
                foreach (var element in root.Elements())
                {
                    result[element.Name.LocalName] = element.Value;
                }
            }
        }
        catch (Exception)
        {
            // 如果解析失败，返回空字典
        }

        return result;
    }

    /// <summary>
    /// 将字典转换为 XML 字符串
    /// </summary>
    public static string DictionaryToXml(Dictionary<string, string> data)
    {
        var xml = new XElement("xml");

        foreach (var kvp in data)
        {
            if (!string.IsNullOrEmpty(kvp.Value))
            {
                xml.Add(new XElement(kvp.Key, new XCData(kvp.Value)));
            }
        }

        return xml.ToString();
    }

    /// <summary>
    /// 将对象转换为 XML 字符串（用于微信支付请求）
    /// </summary>
    public static string ObjectToXml(object data, string? sign = null)
    {
        var xml = new XElement("xml");

        foreach (var prop in data.GetType().GetProperties())
        {
            var value = prop.GetValue(data)?.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                xml.Add(new XElement(prop.Name, new XCData(value)));
            }
        }

        if (!string.IsNullOrEmpty(sign))
        {
            xml.Add(new XElement("sign", new XCData(sign)));
        }

        return xml.ToString();
    }
}

