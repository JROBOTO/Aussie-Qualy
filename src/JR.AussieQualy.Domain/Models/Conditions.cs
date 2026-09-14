namespace JR.AussieQualy.Domain.Models;

public sealed class Conditions
{
    public double AirTemperature { get; init; }
    public double Humidity { get; init; }
    public double Pressure { get; init; }
    public double Rainfall { get; init; }
    public double TrackTemperature { get; init; }
    public double WindDirectionBearing { get; init; }
    public double WindSpeedInMs { get; init; }
}
