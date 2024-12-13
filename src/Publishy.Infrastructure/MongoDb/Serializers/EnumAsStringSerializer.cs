using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace Publishy.Infrastructure.MongoDb.Serializers;

public class EnumAsStringSerializer<TEnum> : EnumSerializer<TEnum> where TEnum : struct, Enum
{
    public EnumAsStringSerializer() : base(BsonType.String) { }
}
