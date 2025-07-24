using MongoDB.Driver;

namespace ECS_Logistics.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration config)
    {
        var connectionString = config.GetConnectionString("MongoDB");
        var mongoUrl = new MongoUrl(connectionString);
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(mongoUrl.DatabaseName);
    }
}