namespace Publishy.Application.Domain.ValueObjects;

public record MediaAsset
{
    public string Url { get; init; }
    public string Type { get; init; }
    public string? AltText { get; init; }

    public MediaAsset(string url, string type, string? altText = null)
    {
        Url = url;
        Type = type;
        AltText = altText;
    }
}