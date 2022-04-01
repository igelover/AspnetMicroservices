using AutoMapper;
using EventBus.Messages.Events;
using Ordering.Application.Models;

namespace Ordering.API.Mapping
{
    public class OrderingProfile : Profile
    {
        public OrderingProfile()
        {
            CreateMap<OrderDTO, BasketCheckoutEvent>().ReverseMap();
        }
    }
}
