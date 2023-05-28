using Data.MongoDbSettings;
using MongoDB.Driver;

namespace Data.Context
{
    public interface IMongoContext
    {
        IMongoDatabase Database { get; }
    }
    

    public class MongoContext : IMongoContext
    {
        private readonly IMongoDatabase _database;
        
        
        private void CreateDatabaseIfNotExists()
        {
            var databaseNames = _database.Client.ListDatabaseNames().ToList();
            var databaseName = _database.DatabaseNamespace.DatabaseName;

            if (!databaseNames.Contains(databaseName))
            {
                _database.Client.GetDatabase(databaseName);
            }
        }

        
        public MongoContext(IMongoDbSettings connectionSetting)
        {
            var client = new MongoClient(connectionSetting.ConnectionString);
            _database = client.GetDatabase(connectionSetting.DatabaseName);
            
            CreateDatabaseIfNotExists();
        }

        public IMongoDatabase Database => _database;
    }
    
}