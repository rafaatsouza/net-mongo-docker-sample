namespace MongoDockerSample.Infrastructure.Repository
{
    public class RepositoryConfiguration
    {
        public string MongoServer { get; set; }
        public string MongoDatabase { get; set; }
        public string MongoCollection { get; set; }
    }
}