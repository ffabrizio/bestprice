using Microsoft.Azure.Cosmos.Fluent;

namespace Laerdal.BestPrice.Repository
{
    public static class CosmosInitializer
    {
        public static ICosmosRepository Initialize(CosmosSettings settings)
        {
            var clientBuilder = new CosmosClientBuilder(settings.Account, settings.Key);
            var client = clientBuilder.WithConnectionModeDirect().Build();
            var repository = new CosmosRepository(client, settings.DatabaseName, settings.ContainerName);
            var database = client.CreateDatabaseIfNotExistsAsync(settings.DatabaseName).GetAwaiter().GetResult();
            
            database.Database.CreateContainerIfNotExistsAsync(settings.ContainerName, "/pk", 1000)
                .GetAwaiter()
                .GetResult();

            return repository;
        }
    }
}
