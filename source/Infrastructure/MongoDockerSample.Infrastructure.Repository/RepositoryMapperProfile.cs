using AutoMapper;
using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Infrastructure.Repository.Dtos;

namespace MongoDockerSample.Infrastructure.Repository
{
    public class RepositoryMapperProfile : Profile
    {
        public RepositoryMapperProfile()
        {
            CreateMap<RegisterDto, Register>();
        }
    }
}