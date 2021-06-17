using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistance
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                await orderContext.Orders.AddRangeAsync(GetOrdersToSeed());
                await orderContext.SaveChangesAsync();
            }
        }

        private static IEnumerable<Order> GetOrdersToSeed()
        {
            return new List<Order>
            {
                new Order { UserName = "admin", FirstName = "Admin", LastName = "Admin", EmailAddress = "admin@email.com", AddressLine = "Reforma 123", Country = "Mexico", TotalPrice = 350}
            };
        }
    }
}
