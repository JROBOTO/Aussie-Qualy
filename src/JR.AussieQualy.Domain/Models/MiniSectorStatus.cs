namespace JR.AussieQualy.Domain.Models;

public sealed class MiniSectorStatus
{
    public IReadOnlyList<string> MiniSectors { get; init; } = Array.Empty<string>();
}
