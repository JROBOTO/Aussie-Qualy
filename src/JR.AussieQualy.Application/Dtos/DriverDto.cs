namespace JR.AussieQualy.Application.Dtos;

public sealed class DriverDto
{
    public string Code { get; init; } = default!;
    public string Name { get; init; } = default!;
    public int DriverNumber { get; init; }
    public string Team { get; init; } = default!;
}
