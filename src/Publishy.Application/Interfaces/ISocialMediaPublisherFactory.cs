namespace Publishy.Application.Interfaces;

public interface ISocialMediaPublisherFactory
{
    ISocialMediaPublisher GetPublisher(string platform);
}