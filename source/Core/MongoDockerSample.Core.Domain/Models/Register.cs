using System;

namespace MongoDockerSample.Core.Domain.Models
{
    public class Register
    {
        public Guid Key { get; set; }

        public string Value { get; set; }
    }
}