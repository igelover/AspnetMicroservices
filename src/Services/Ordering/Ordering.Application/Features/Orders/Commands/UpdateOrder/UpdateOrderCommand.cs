using MediatR;
using Ordering.Application.Models;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommand : IRequest
    {
        public OrderDTO Order { get; set; }
    }
}
