using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Scripts;
using System.IO;
using System.Reflection;

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

            var container = database.Database.CreateContainerIfNotExistsAsync(settings.ContainerName, Constants.PkPath, 1000)
                .GetAwaiter()
                .GetResult();

            var cosmosScripts = container.Container.Scripts;

            try
            {
                cosmosScripts.DeleteStoredProcedureAsync(Constants.SpId).GetAwaiter().GetResult();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                //Nothing to delete
            }

            cosmosScripts.CreateStoredProcedureAsync(new StoredProcedureProperties(Constants.SpId, ReadSpResource(Constants.SpId)))
               .GetAwaiter()
               .GetResult();

            return repository;
        }

        private static string ReadSpResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Repository.{name}.js"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
