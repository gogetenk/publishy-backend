namespace Publishy.Application.Domain.ValueObjects;

public record ContentStrategy(
    string Platform,
    string ContentType,
    string Frequency,
    string TargetAudience,
    string ToneOfVoice,
    List<string> KeyTopics
);