using System;
using AudioStudy.Bot.Domain.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace AudioStudy.Bot.DataAccess.Db
{
    public class MongoDbContext
    {
        static MongoDbContext()
        {
            ClassMaps.Register();
            BsonSerializer.RegisterSerializer(typeof(DateTime), new CustomDateTimeSerializer());
            ConventionPack conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
        }
        
        private const string UserCollectionName = "User";
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<DbOptions> config)
        {
            var client = new MongoClient(config.Value.ConnectionString);
            _database = client.GetDatabase(config.Value.DatabaseName);
        }
        
        public virtual IMongoCollection<User> Users => _database.GetCollection<User>(UserCollectionName);
    }
}