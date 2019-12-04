namespace Laerdal.BestPrice.Repository
{
    /*
     * When running locally (either from the Azure Cosmos Emulator or an Azure Subscription, 
     * these settings are bound to a "CosmosDbSettings" top-level node in the local.settings.json file, 
     * e.g.:
     * 
     * "CosmosDbSettings": {
     *  "databaseName": "<db name>",
     *  "containerName": "<collection name>",
     *  "account": "<endpoint url>",
     *  "key": "<secret key>"
     * }
    */

    public class CosmosSettings
    {
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
        public string Account { get; set; }
        public string Key { get; set; }
    }
}
