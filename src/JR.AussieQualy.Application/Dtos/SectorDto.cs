namespace JR.AussieQualy.Application.Dtos;

public sealed class SectorDto
{
    public double? SectorTime { get; init; }
    public IReadOnlyList<string> MiniSectors { get; init; } = Array.Empty<string>();
    public string? SectorStatus { get; init; }
}
