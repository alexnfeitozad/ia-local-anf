using IDEIANF.Local.Domain.Projects;

namespace IDEIANF.Local.Application.Services;

public interface IWorkspaceService
{
    string? CurrentWorkspaceRoot { get; }
    ProjectInfo? CurrentProject { get; }

    bool TrySetCurrentWorkspaceRoot(string workspaceRoot);
    ProjectInfo Inspect(string workspaceRoot);
    IReadOnlyList<string> GetTopLevelEntries(string workspaceRoot);
    bool IsWithinWorkspace(string candidatePath);
}
