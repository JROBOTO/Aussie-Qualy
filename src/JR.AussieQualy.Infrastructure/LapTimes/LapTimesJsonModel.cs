namespace JR.AussieQualy.Infrastructure.LapTimes;

public sealed class LapTimesJsonModel
{
    public List<double?> time { get; init; } = new();
    public List<int> lap { get; init; } = new();
    public List<string> compound { get; init; } = new();
    public List<int> stint { get; init; } = new();
    public List<double?> s1 { get; init; } = new();
    public List<double?> s2 { get; init; } = new();
    public List<double?> s3 { get; init; } = new();
    public List<string> ms1 { get; init; } = new();
    public List<string> ms2 { get; init; } = new();
    public List<string> ms3 { get; init; } = new();

    public List<string> driverCode { get; init; } = new();
    public List<string> driverName { get; init; } = new();
    public List<int?> driverNumber { get; init; } = new();
    public List<string> team { get; init; } = new();

    public List<double?> airTemp { get; init; } = new();
    public List<double?> humidity { get; init; } = new();
    public List<double?> pressure { get; init; } = new();
    public List<double?> rainfall { get; init; } = new();
    public List<double?> trackTemp { get; init; } = new();
    public List<double?> windDir { get; init; } = new();
    public List<double?> windSpeed { get; init; } = new();

    public List<int?> pos { get; init; } = new();
    public List<string> qSegment { get; init; } = new();
    public List<double> lapStartTime { get; init; } = new();
}
