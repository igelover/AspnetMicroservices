using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistance;
using Ordering.Application.Exceptions;
using Ordering.Application.Models;
using Ordering.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(IOrderRepository repository, IMapper mapper, ILogger<UpdateOrderCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _repository.GetByIdAsync(request.Order.Id);
            if (orderToUpdate is null)
            {
                _logger.LogError("Order {orderId} does not exist in database.", request.Order.Id);
                throw new NotFoundException(nameof(Order), request.Order.Id);
            }

            _mapper.Map(request.Order, orderToUpdate, typeof(OrderDTO), typeof(Order));
            await _repository.UpdateAsync(orderToUpdate);
            _logger.LogInformation("Order {orderId} was successfully updated.", orderToUpdate.Id);
            return Unit.Value;
        }
    }
}
