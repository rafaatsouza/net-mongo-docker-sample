using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;

namespace MongoDockerSample.Infrastructure.Repository.Dtos
{
    internal class RecordDto
    {
        [BsonId(IdGenerator = typeof(GuidGenerator)), BsonRepresentation(BsonType.String)]
        public Guid Key { get; set; }

        public string Value { get; set; }

        public RecordDto(string value)
        {
            Key = Guid.NewGuid();
            Value = value;
        }
    }
}