namespace Publishy.Domain.Analytics;

public class NetworkDistribution
{
    public string Network { get; private set; }
    public MediaTypePercentages Percentages { get; private set; }

    public NetworkDistribution(string network, MediaTypePercentages percentages)
    {
        Network = network;
        Percentages = percentages;
    }
}

public class MediaTypePercentages
{
    public float Text { get; private set; }
    public float Image { get; private set; }
    public float Video { get; private set; }

    public MediaTypePercentages(float text, float image, float video)
    {
        Text = text;
        Image = image;
        Video = video;
    }
}