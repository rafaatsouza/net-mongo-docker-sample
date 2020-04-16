using System;

namespace MongoDockerSample.Core.Domain.Models
{
    public class Record
    {
        public Guid Key { get; set; }

        public string Value { get; set; }
    }
}