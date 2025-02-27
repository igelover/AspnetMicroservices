﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging
{
    public static class SeriLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure()
        {
            return (context, configuration) =>
            {
                var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
                var indexFormat = $"applogs" +
                    $"-" +
                    $"{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}" +
                    $"-" +
                    $"{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}" +
                    $"-" +
                    $"{DateTime.UtcNow:yyyy-MM}";

                configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Debug()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(
                        new ElasticsearchSinkOptions(new Uri(elasticUri!))
                        {
                            IndexFormat = indexFormat,
                            AutoRegisterTemplate = true,
                            NumberOfShards = 2,
                            NumberOfReplicas = 1
                        }
                    )
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .ReadFrom.Configuration(context.Configuration);
            };
        }
    }
}
