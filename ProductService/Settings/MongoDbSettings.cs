namespace ProductService.Settings
{
    public class MongoDbSettings
    {
        // ✅ required supprime les warnings CS8618
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
    }
}