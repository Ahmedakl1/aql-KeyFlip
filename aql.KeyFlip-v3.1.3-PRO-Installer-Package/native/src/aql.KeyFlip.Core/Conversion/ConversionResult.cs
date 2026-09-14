namespace aql.KeyFlip.Core.Conversion;

public record ConversionResult
{
    public required string Original { get; init; }
    public required string Converted { get; init; }
    public required string Direction { get; init; } // "EN_TO_AR" or "AR_TO_EN"
    public int WordsCount { get; init; }
    public int CharsCount { get; init; }
    public double Confidence { get; init; } = 1.0;
    public bool IsChanged => !string.Equals(Original, Converted, System.StringComparison.Ordinal);
}
