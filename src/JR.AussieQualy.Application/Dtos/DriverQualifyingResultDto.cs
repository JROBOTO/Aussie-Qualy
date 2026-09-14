namespace JR.AussieQualy.Application.Dtos;

public sealed class DriverQualifyingResultDto
{
    public DriverDto Driver { get; init; } = default!;
    public int FinalQualifyingPosition { get; init; }
    public LapDataDto? Q1 { get; init; }
    public LapDataDto? Q2 { get; init; }
    public LapDataDto? Q3 { get; init; }
}
