using System;

namespace MongoDockerSample.Core.Domain.Models
{
    public record Entry(Guid Key, string Value);
}