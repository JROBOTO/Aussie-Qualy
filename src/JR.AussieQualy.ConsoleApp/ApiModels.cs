namespace JR.AussieQualy.ConsoleApp;

public sealed class DriverDto
{
    public string Code { get; init; } = default!;
    public string Name { get; init; } = default!;
    public int DriverNumber { get; init; }
    public string Team { get; init; } = default!;
}

public sealed class DriverQualifyingResultDto
{
    public DriverDto Driver { get; init; } = default!;
    public int FinalQualifyingPosition { get; init; }
    public LapDataDto? Q1 { get; init; }
    public LapDataDto? Q2 { get; init; }
    public LapDataDto? Q3 { get; init; }
}

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

public sealed class SectorDto
{
    public double? SectorTime { get; init; }
    public IReadOnlyList<string> MiniSectors { get; init; } = Array.Empty<string>();
    public string? SectorStatus { get; init; }
}

public sealed class TireDto
{
    public string Compound { get; init; } = default!;
    public int Life { get; init; }
}

public sealed class ConditionsDto
{
    public double? AirTemperature { get; init; }
    public double? Humidity { get; init; }
    public double? Pressure { get; init; }
    public double? Rainfall { get; init; }
    public double? TrackTemperature { get; init; }
    public double? WindDirectionBearing { get; init; }
    public double? WindSpeedInMs { get; init; }
}
