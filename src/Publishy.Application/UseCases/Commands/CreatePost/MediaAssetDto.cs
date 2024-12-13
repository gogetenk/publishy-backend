namespace Publishy.Application.UseCases.Commands.CreatePost;

public class MediaAssetDto
{
    public string Url { get; set; }
    public string Type { get; set; }
    public string? AltText { get; set; }

    public MediaAssetDto(string url, string type, string? altText = null)
    {
        Url = url;
        Type = type;
        AltText = altText;
    }
}
