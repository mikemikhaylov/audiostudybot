using AudioStudy.Bot.Domain.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace AudioStudy.Bot.DataAccess.Db
{
    public class ClassMaps
    {
        public static void Register()
        {
            BsonClassMap.RegisterClassMap<User>(x =>
            {
                x.AutoMap();
                x.MapIdMember(xx => xx.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId))
                    .SetIgnoreIfDefault(true);
                x.SetIgnoreExtraElements(true);
            });
        }
    }
}