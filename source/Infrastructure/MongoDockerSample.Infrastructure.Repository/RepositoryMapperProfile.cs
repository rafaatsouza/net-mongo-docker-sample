using AutoMapper;
using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Infrastructure.Repository.Dtos;

namespace MongoDockerSample.Infrastructure.Repository
{
    internal class RepositoryMapperProfile : Profile
    {
        public RepositoryMapperProfile()
        {
            CreateMap<RecordDto, Record>();
        }
    }
}