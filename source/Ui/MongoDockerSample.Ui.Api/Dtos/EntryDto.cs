using System;

namespace MongoDockerSample.Ui.Api.Dtos
{
    public class EntryDto
    {
        public Guid Key { get; set; }

        public string Value { get; set; }
    }
}