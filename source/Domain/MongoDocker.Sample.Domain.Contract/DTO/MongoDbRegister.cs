using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;

namespace MongoDocker.Sample.Domain.Contract.DTO
{
    public class MongoDbRegister
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Key { get; set; }

        public string Value { get; set; }

        public MongoDbRegister(string value)
        {
            Key = Guid.NewGuid();
            Value = value;
        }
    }
}