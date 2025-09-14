using AutoMapper;
using MilitaryOperationAPI.Domain.Models.Entities;
using MilitaryOperationAPI.Domain.Models.RequestModels;

namespace MilitaryOperationAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<LoginInputModel, User>().ReverseMap();
            CreateMap<RegisterInputModel, User>().ReverseMap();
            CreateMap<OperationInputModel, Operation>().ReverseMap();
        }
    }
}
