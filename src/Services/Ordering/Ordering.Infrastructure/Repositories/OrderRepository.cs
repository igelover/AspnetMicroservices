﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistance;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistance;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(IDbContextFactory<OrderContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByUsernameAsync(string username)
        {
            return await GetAsync(o => o.UserName == username);
        }
    }
}
