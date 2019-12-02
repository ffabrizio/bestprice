using Laerdal.BestPrice;
using Laerdal.BestPrice.Calculators;
using Laerdal.BestPrice.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Json.Internal;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Laerdal.BestPrice
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var executionContextOptions = builder.Services.BuildServiceProvider().GetService<IOptions<ExecutionContextOptions>>().Value;
            var appDirectory = executionContextOptions.AppDirectory;

            var config = new ConfigurationBuilder()
                .SetBasePath(appDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var cosmosSettings = new CosmosSettings();
            config.GetSection("CosmosDbSettings").Bind(cosmosSettings);
            var cosmosInstance = CosmosInitializer.Initialize(cosmosSettings);
            
            builder.Services.AddSingleton(cosmosInstance);
            builder.Services.AddSingleton<ICalculator, Calculator>();
            builder.Services.AddTransient<IConfigureOptions<MvcOptions>, MvcJsonMvcOptionsSetup>();
        }
    }
}
