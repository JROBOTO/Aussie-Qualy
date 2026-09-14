namespace JR.AussieQualy.Domain.Models
{
    public sealed class TireInfo
    {
        public string Compound { get; init; } = default!;
        public int Life { get; init; }
    }
}
