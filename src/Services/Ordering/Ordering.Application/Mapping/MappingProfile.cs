using AutoMapper;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDTO>().ReverseMap();
        }
    }
}
