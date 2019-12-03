namespace Laerdal.BestPrice
{
    public static class Constants
    {
        // Catalog properties
        public const string Sku = "Sku";
        public const string ProductGroup = "ProductGroup";
        public const string ProductLine = "ProductLine";
        public const string ProductType = "ProductType";

        // Cosmos constants
        /// <summary>
        /// The path used as Partition Key in cosmos entities
        /// </summary>
        public const string PkPath = "/pk";
        /// <summary>
        /// The id of the stored procedure used for bulk deletion by Partition Key
        /// </summary>
        public const string SpId = "BulkDeletePk";
    }
}
