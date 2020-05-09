using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Infrastructure.Repository.Dtos;

namespace AutoMapper
{
    public class RepositoryMapperProfile : Profile
    {
        public RepositoryMapperProfile()
        {
            CreateMap<RecordDto, Record>();
        }
    }
}