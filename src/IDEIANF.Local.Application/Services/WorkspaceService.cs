using System.IO;
using IDEIANF.Local.Domain.Projects;

namespace IDEIANF.Local.Application.Services;

public sealed class WorkspaceService : IWorkspaceService
{
    public string? CurrentWorkspaceRoot { get; private set; }
    public ProjectInfo? CurrentProject { get; private set; }

    public bool TrySetCurrentWorkspaceRoot(string workspaceRoot)
    {
        if (string.IsNullOrWhiteSpace(workspaceRoot))
        {
            return false;
        }

        var fullPath = Path.GetFullPath(workspaceRoot);
        if (!Directory.Exists(fullPath))
        {
            return false;
        }

        CurrentWorkspaceRoot = fullPath;
        CurrentProject = Inspect(fullPath);
        return true;
    }

    public ProjectInfo Inspect(string workspaceRoot)
    {
        if (string.IsNullOrWhiteSpace(workspaceRoot))
        {
            throw new ArgumentException("Workspace root cannot be empty.", nameof(workspaceRoot));
        }

        var fullPath = Path.GetFullPath(workspaceRoot);
        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"Workspace not found: {fullPath}");
        }

        var topLevelEntries = Directory
            .EnumerateFileSystemEntries(fullPath)
            .Where(entry => !entry.Contains(".git", StringComparison.OrdinalIgnoreCase))
            .Where(entry => !entry.Contains("bin", StringComparison.OrdinalIgnoreCase))
            .Where(entry => !entry.Contains("obj", StringComparison.OrdinalIgnoreCase))
            .OrderBy(entry => Path.GetFileName(entry), StringComparer.OrdinalIgnoreCase)
            .Select(entry => Path.GetFileName(entry) ?? entry)
            .Take(25)
            .ToArray();

        var solutionFile = Directory
            .EnumerateFiles(fullPath, "*.sln", SearchOption.TopDirectoryOnly)
            .Concat(Directory.EnumerateFiles(fullPath, "*.slnx", SearchOption.TopDirectoryOnly))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        return new ProjectInfo(
            Name: Path.GetFileName(fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) ?? fullPath,
            RootPath: fullPath,
            SolutionFile: solutionFile is null ? null : Path.GetFileName(solutionFile),
            TopLevelEntries: topLevelEntries,
            DiscoveredAt: DateTimeOffset.Now);
    }

    public IReadOnlyList<string> GetTopLevelEntries(string workspaceRoot)
    {
        return Inspect(workspaceRoot).TopLevelEntries;
    }

    public bool IsWithinWorkspace(string candidatePath)
    {
        if (string.IsNullOrWhiteSpace(candidatePath) || string.IsNullOrWhiteSpace(CurrentWorkspaceRoot))
        {
            return false;
        }

        var fullCandidate = Path.GetFullPath(candidatePath);
        var workspaceRoot = Path.GetFullPath(CurrentWorkspaceRoot);

        return fullCandidate.StartsWith(workspaceRoot, StringComparison.OrdinalIgnoreCase);
    }
}
