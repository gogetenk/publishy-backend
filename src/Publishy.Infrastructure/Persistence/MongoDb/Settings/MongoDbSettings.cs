namespace Publishy.Infrastructure.Persistence.MongoDb.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}