using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistance;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _repository;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IOrderRepository repository, IMapper mapper, ILogger<DeleteOrderCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        async Task IRequestHandler<DeleteOrderCommand>.Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await _repository.GetByIdAsync(request.Id);
            if (orderToDelete is null)
            {
                _logger.LogError("Order {orderId} does not exist in database.", request.Id);
                throw new NotFoundException(nameof(Order), request.Id);
            }

            await _repository.DeleteAsync(orderToDelete);
            _logger.LogInformation("Order {orderId} was successfully deleted.", orderToDelete.Id);
        }
    }
}
