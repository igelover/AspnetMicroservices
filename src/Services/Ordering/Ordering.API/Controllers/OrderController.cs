﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrderList;
using Ordering.Application.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{username}", Name = "GetOrders")]
        [ProducesResponseType(typeof(IEnumerable<OrderDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByUsername(string username)
        {
            var query = new GetOrdersListQuery(username);
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }

        //[HttpPost(Name = "CheckoutOrder")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //public async Task<ActionResult<int>> CheckoutOrder([FromBody] OrderDTO order)
        //{
        //    var command = new CheckoutOrderCommand { Order = order };
        //    var result = await _mediator.Send(command);
        //    return Ok(result);
        //}

        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<int>> UpdateOrder([FromBody] OrderDTO order)
        {
            var command = new UpdateOrderCommand { Order = order };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete(Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<int>> DeleteOrder(int id)
        {
            var command = new DeleteOrderCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
