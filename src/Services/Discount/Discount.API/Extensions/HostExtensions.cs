﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using System;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var maxRetries = 5;

                try
                {
                    logger.LogInformation("Migrating the PostgreSQL database.");

                    var retry = Policy.Handle<NpgsqlException>()
                        .WaitAndRetry(
                            retryCount: maxRetries,
                            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            onRetry: (exception, timeSpan, retry, context) =>
                            {
                                logger.LogError(exception,
                                    "[{prefix}] {ExceptionType}: {Message} on attempt {retry} of {retries}",
                                    typeof(TContext).Name,
                                    exception.GetType().Name,
                                    exception.Message,
                                    retry,
                                    maxRetries);
                            });

                    retry.Execute(() => ExecuteMigrations(configuration));

                    logger.LogInformation("Migrated the PostgreSQL database.");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the PostgreSQL database.");
                }
            }
            return host;
        }

        private static void ExecuteMigrations(IConfiguration configuration)
        {
            using var connection = new NpgsqlConnection(
                configuration.GetValue<string>("DatabaseSettings:ConnectionString")
            );
            connection.Open();
            
            using var command = new NpgsqlCommand
            {
                Connection = connection
            };
            
            command.CommandText = "DROP TABLE IF EXISTS Coupon";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE Coupon(
                                                Id SERIAL PRIMARY KEY,
                                                ProductName VARCHAR(24) NOT NULL,
                                                Description TEXT,
                                                Amount INT)";
            command.ExecuteNonQuery();

            command.CommandText = @"INSERT INTO Coupon
                                                (ProductName, Description, Amount)
                                            VALUES
                                                ('IPhone X', 'IPhone Discount', 150);";
            command.ExecuteNonQuery();

            command.CommandText = @"INSERT INTO Coupon
                                                (ProductName, Description, Amount) 
                                            VALUES
                                                ('Samsung 10', 'Samsung Discount', 100);";
            command.ExecuteNonQuery();
        }
    }
}
