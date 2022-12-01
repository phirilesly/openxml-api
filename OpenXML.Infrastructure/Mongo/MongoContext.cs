using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using OpenXML.Abstractions.Models;

namespace OpenXML.Infrastructure.Mongo
{
    public class MongoContext
    {
        public IMongoDatabase Database { get; }
        private readonly string _serverName;
        private readonly string _databaseName;

        private readonly ConventionPack camelConventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
        private readonly ConventionPack ignoreExtraElementsPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
        private readonly ConventionPack ignoreNullsPack = new ConventionPack { new IgnoreIfNullConvention(true) };
        private readonly MongoClient client;
        public string ServerName => _serverName;
        public string DatabaseName => _databaseName;

        public MongoContext(string connectionString)
        {
            ConventionRegistry.Register("CamelCaseConvensions", camelConventionPack, t => true);
            ConventionRegistry.Register("IgnoreExtraElements", ignoreExtraElementsPack, t => true);
            ConventionRegistry.Register("Ignore null values", ignoreNullsPack, t => true);
            client = new MongoClient(connectionString);
            Database = client.GetDatabase(MongoUrl.Create(connectionString).DatabaseName);
        }
        public MongoContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ReportsTemplates");
            ConventionPack pack = new ConventionPack
            {
                new IgnoreIfNullConvention(true),
                 new IgnoreExtraElementsConvention(true),
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("defaults", pack, t => true);
            client = new MongoClient(connectionString);
            Database = client.GetDatabase(MongoUrl.Create(connectionString).DatabaseName);
        }

        public MongoContext(string serverName, string databaseName)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            MongoClient client = new MongoClient(_serverName);
            ConventionRegistry.Register("CamelCaseConvensions", camelConventionPack, t => true);
            ConventionRegistry.Register("IgnoreExtraElements", ignoreExtraElementsPack, t => true);
            ConventionRegistry.Register("Ignore null values", ignoreNullsPack, t => true);
            Database = client.GetDatabase(_databaseName);
        }

        public IMongoCollection<Section> Sections => Database.GetCollection<Section>("Sections");
        public IMongoCollection<Template> Templates => Database.GetCollection<Template>("Templates");

    }
}
