using IDEIANF.Local.Application.Services;

namespace IDEIANF.Local.Tests;

public class WorkspaceServiceTests
{
    [Fact]
    public void TrySetCurrentWorkspaceRoot_WithExistingDirectory_SetsCurrentWorkspace()
    {
        var workspaceRoot = Path.GetTempPath();
        var service = new WorkspaceService();

        var result = service.TrySetCurrentWorkspaceRoot(workspaceRoot);

        Assert.True(result);
        Assert.Equal(Path.GetFullPath(workspaceRoot), service.CurrentWorkspaceRoot);
        Assert.NotNull(service.CurrentProject);
    }

    [Fact]
    public void IsWithinWorkspace_WithNestedPath_ReturnsTrue()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        var nestedPath = Path.Combine(tempRoot, "src", "app");
        Directory.CreateDirectory(nestedPath);

        var service = new WorkspaceService();
        service.TrySetCurrentWorkspaceRoot(tempRoot);

        var result = service.IsWithinWorkspace(nestedPath);

        Assert.True(result);

        Directory.Delete(tempRoot, recursive: true);
    }

    [Fact]
    public void TrySetCurrentWorkspaceRoot_WithMissingDirectory_ReturnsFalse()
    {
        var service = new WorkspaceService();

        var result = service.TrySetCurrentWorkspaceRoot(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));

        Assert.False(result);
    }
}