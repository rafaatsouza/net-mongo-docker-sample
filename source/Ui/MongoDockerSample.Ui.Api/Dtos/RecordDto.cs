using System;

namespace MongoDockerSample.Ui.Api.Dtos
{
    public class RecordDto
    {
        public Guid Key { get; set; }

        public string Value { get; set; }
    }
}