namespace PersonalSite.Api.Services.Reading;

public class ReadingLibraryOptions
{
    public const string SectionName = "ReadingLibrary";
    // Explicit stable identity. Missing security configuration disables reading access.
    public string OwnerKey { get; set; } = "";
    public string AccessPasswordHash { get; set; } = "";
    public string[] AllowedOrigins { get; set; } = [];
    public int SessionDays { get; set; } = 30;
    public int AbsoluteSessionDays { get; set; } = 90;
    public string StorageRoot { get; set; } = "App_Data/reading-private";
    public string CalibreExecutable { get; set; } = "";
    public int ConversionTimeoutSeconds { get; set; } = 120;
    public long ConversionMemoryBytes { get; set; } = 1024L * 1024 * 1024;
    public const long MaxUploadBytes = 100 * 1024 * 1024;
    public long MaxExpandedBytes { get; set; } = 100 * 1024 * 1024;
    public long MaxEntryBytes { get; set; } = 10 * 1024 * 1024;
    public int MaxEntries { get; set; } = 2000;
    public int MaxCompressionRatio { get; set; } = 100;
}

public class ReadingLibraryException(int statusCode, string code, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string Code { get; } = code;
}
