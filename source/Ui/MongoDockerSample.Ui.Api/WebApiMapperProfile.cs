using AutoMapper;
using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Ui.Api.Dtos;

namespace MongoDockerSample.Ui.Api
{
    public class WebApiMapperProfile : Profile
    {
        public WebApiMapperProfile()
        {
            CreateMap<Entry, EntryDto>();
        }
    }
}