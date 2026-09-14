namespace JR.AussieQualy.Domain.Models;

public sealed class Driver
{
    public string Code { get; init; } = default!;
    public string Name { get; init; } = default!;
    public int Number { get; init; }
    public string Team { get; init; } = default!;
}
