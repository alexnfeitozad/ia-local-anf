namespace IDEIANF.Local.Domain.Projects;

public sealed record ProjectInfo(
    string Name,
    string RootPath,
    string? SolutionFile,
    IReadOnlyList<string> TopLevelEntries,
    DateTimeOffset DiscoveredAt);
