using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace PersonalSite.Api.Services.Reading;

// Local private storage uses existing ASP.NET file APIs. Nothing is mapped as static content.
public sealed class PrivateBookStorage
{
    private readonly string _root;
    private readonly string _contentRoot;

    public PrivateBookStorage(IWebHostEnvironment environment, IOptions<ReadingLibraryOptions> options)
    {
        _contentRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(environment.ContentRootPath));
        _root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(options.Value.StorageRoot, environment.ContentRootPath));
        // ContentRoot/../.. is the monorepo root in local checkout; on a flat publish
        // (e.g. /var/www/personal_site_api) it becomes /var — do not treat that as "project".
        var candidateProjectRoot = Path.GetFullPath(Path.Combine(_contentRoot, "../.."));
        var monorepoRoot = LooksLikeMonorepo(candidateProjectRoot) ? candidateProjectRoot : null;
        var appDataRoot = Path.Combine(_contentRoot, "App_Data");
        var forbiddenRoots = new List<string>
        {
            environment.WebRootPath ?? Path.Combine(_contentRoot, "wwwroot"),
            Path.Combine(_contentRoot, "wwwroot"),
        };
        if (monorepoRoot != null)
        {
            forbiddenRoots.Add(Path.Combine(monorepoRoot, "public"));
            forbiddenRoots.Add(Path.Combine(monorepoRoot, ".output"));
            forbiddenRoots.Add(Path.Combine(monorepoRoot, "content"));
            forbiddenRoots.Add(Path.Combine(monorepoRoot, ".git"));
        }
        if (forbiddenRoots.Any(p => ContainsPath(p, _root) || ContainsPath(_root, p)))
            throw new InvalidOperationException("ReadingLibrary storage must be outside public/content/build roots.");
        // Under the publish/content root, only App_Data is allowed.
        if (ContainsPath(_contentRoot, _root) && !ContainsPath(appDataRoot, _root))
            throw new InvalidOperationException("Within the app content root, private storage must be inside App_Data.");
        // Under a detected monorepo (local checkout), still require App_Data — not sibling folders.
        if (monorepoRoot != null && ContainsPath(monorepoRoot, _root) && !ContainsPath(appDataRoot, _root))
            throw new InvalidOperationException("Within the project, private storage must be inside App_Data.");
        EnsureNoLinks(_root);
    }

    /// <summary>Resolved absolute private root (never expose over HTTP).</summary>
    public string RootPath => _root;

    /// <summary>True when books live under the app publish folder (risky: redeploy can wipe them).</summary>
    public bool IsInsideContentRoot => ContainsPath(_contentRoot, _root);

    /// <summary>Create the private directory if missing; call once at process start for production StorageRoot.</summary>
    public void EnsureReady()
    {
        EnsureNoLinks(_root);
        if (OperatingSystem.IsWindows()) Directory.CreateDirectory(_root);
        else Directory.CreateDirectory(_root, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        EnsureNoLinks(_root);
    }

    private static bool LooksLikeMonorepo(string candidateRoot)
    {
        return File.Exists(Path.Combine(candidateRoot, "nuxt.config.ts"))
            || File.Exists(Path.Combine(candidateRoot, "nuxt.config.js"))
            || Directory.Exists(Path.Combine(candidateRoot, ".git"));
    }

    private static bool ContainsPath(string parent, string child)
    {
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        parent = Path.GetFullPath(parent).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        child = Path.GetFullPath(child).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        return child.StartsWith(parent, comparison);
    }

    private static void EnsureNoLinks(string path)
    {
        for (var current = new DirectoryInfo(path); current != null; current = current.Parent)
            if (current.Exists && (current.Attributes & FileAttributes.ReparsePoint) != 0)
                throw new IOException("ReadingLibrary storage cannot use symbolic links or junctions.");
    }

    private string Resolve(string key)
    {
        if (!Regex.IsMatch(key, "^[a-f0-9]{32}\\.(epub|tmp)$", RegexOptions.CultureInvariant))
            throw new IOException("Invalid private storage key.");
        EnsureNoLinks(_root);
        var path = Path.Combine(_root, key);
        if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
            throw new IOException("Private files cannot be symbolic links.");
        return path;
    }

    public (string Key, FileStream Stream) CreateTemporary()
    {
        EnsureNoLinks(_root);
        if (OperatingSystem.IsWindows()) Directory.CreateDirectory(_root);
        else Directory.CreateDirectory(_root, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        var key = $"{Guid.NewGuid():N}.tmp";
        return (key, new FileStream(Resolve(key), FileMode.CreateNew, FileAccess.ReadWrite,
            FileShare.None, 81920, FileOptions.Asynchronous));
    }

    public string Commit(string temporaryKey)
    {
        var key = $"{Guid.NewGuid():N}.epub";
        File.Move(Resolve(temporaryKey), Resolve(key), false);
        return key;
    }

    public FileStream Open(string key) => new(Resolve(key), FileMode.Open, FileAccess.Read,
        FileShare.Read, 81920, FileOptions.Asynchronous | FileOptions.SequentialScan);

    public FileStream OpenReadWrite(string key) => new(Resolve(key), FileMode.Open, FileAccess.ReadWrite,
        FileShare.None, 81920, FileOptions.Asynchronous);

    /// <summary>Absolute path for a private key; caller must not expose it over HTTP.</summary>
    public string AbsolutePath(string key) => Resolve(key);

    public void Delete(string key) => File.Delete(Resolve(key));

    public string CreateConversionDirectory()
    {
        EnsureNoLinks(_root);
        var path = Path.Combine(_root, $"{Guid.NewGuid():N}-conversion");
        if (OperatingSystem.IsWindows()) Directory.CreateDirectory(path);
        else Directory.CreateDirectory(path, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        return path;
    }

    public void DeleteConversionDirectory(string path)
    {
        if (!string.Equals(Path.GetDirectoryName(Path.GetFullPath(path)), _root, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)
            || !Regex.IsMatch(Path.GetFileName(path), "^[a-f0-9]{32}-conversion$"))
            throw new IOException("Invalid conversion workspace.");
        EnsureNoLinks(path);
        if (Directory.Exists(path)) Directory.Delete(path, recursive: true);
    }
}
