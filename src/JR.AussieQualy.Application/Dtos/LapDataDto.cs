namespace JR.AussieQualy.Application.Dtos;

public sealed class LapDataDto
{
    public double? LapTime { get; init; }
    public TimeSpan LapStartTime { get; init; }
    public TireDto Tire { get; init; } = default!;
    public ConditionsDto Conditions { get; init; } = default!;
    public int? SegmentPosition { get; init; }
    public SectorDto Sector1 { get; init; } = default!;
    public SectorDto Sector2 { get; init; } = default!;
    public SectorDto Sector3 { get; init; } = default!;
}
