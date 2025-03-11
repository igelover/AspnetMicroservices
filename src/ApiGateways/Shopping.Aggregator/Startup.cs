using System;
using System.Net.Http;
using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using Serilog;
using Shopping.Aggregator.Services;

namespace Shopping.Aggregator
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<LoggingDelegatingHandler>();

            services.AddHttpClient<ICatalogService, CatalogService>(c =>
                c.BaseAddress = new Uri(Configuration["ApiSettings:CatalogUrl"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBrakerPolicy());

            services.AddHttpClient<IBasketService, BasketService>(
                    c => c.BaseAddress = new Uri(Configuration["ApiSettings:BasketUrl"])
                )
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBrakerPolicy());

            services.AddHttpClient<IOrderService, OrderService>(c => 
                c.BaseAddress = new Uri(Configuration["ApiSettings:OrderingUrl"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBrakerPolicy());

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shopping.Aggregator", Version = "v1" });
            });

            services.AddHealthChecks()
                .AddUrlGroup(
                    new Uri($"{Configuration["ApiSettings:CatalogUrl"]}/swagger/index.html"),
                    "Catalog.API Health",
                    HealthStatus.Degraded
                )
                .AddUrlGroup(
                    new Uri($"{Configuration["ApiSettings:BasketUrl"]}/swagger/index.html"),
                    "Basket.API Health",
                    HealthStatus.Degraded
                )
                .AddUrlGroup(
                    new Uri($"{Configuration["ApiSettings:OrderingUrl"]}/swagger/index.html"),
                    "Ordering.API Health",
                    HealthStatus.Degraded
                );
        }

        private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (response, sleepDuration, context) =>
                    {
                        Log.Error($"Waiting {sleepDuration} before re-attempting, due to " +
                            (!string.IsNullOrWhiteSpace(response.Exception?.Message) ?
                            $"exception: {response.Exception?.Message}" :
                            $"StatusCode: {(int)response.Result.StatusCode}, ReasonPhrase: {response.Result.ReasonPhrase}"));
                    });
        }

        private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBrakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping.Aggregator v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}
