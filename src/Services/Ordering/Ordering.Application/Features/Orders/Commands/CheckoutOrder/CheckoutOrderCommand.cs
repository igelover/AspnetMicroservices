using MediatR;
using Ordering.Application.Models;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommand : IRequest<int>
    {
        public OrderDTO Order { get; set; }
    }
}
