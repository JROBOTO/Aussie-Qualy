namespace JR.AussieQualy.Domain.Models;

public sealed class Lap
{
    public Driver Driver { get; init; } = default!;
    public QualifyingSegment Segment { get; init; }
    public int LapNumber { get; init; }
    public double? LapTime { get; init; }
    public TimeSpan LapStartTime { get; init; }
    public TireInfo Tire { get; init; } = default!;
    public Conditions Conditions { get; init; } = default!;
    public int? SegmentPositionAtLapEnd { get; init; }
    public SectorTimes SectorTimes { get; init; } = default!;
    public MiniSectorStatus Sector1Mini { get; init; } = default!;
    public MiniSectorStatus Sector2Mini { get; init; } = default!;
    public MiniSectorStatus Sector3Mini { get; init; } = default!;
}
