using MongoDB.Driver;
using ProductService.Models;
using ProductService.Settings;

namespace ProductService.Services
{
    public class ProductMongoService
    {
        private readonly IMongoCollection<Product> _products;

        public ProductMongoService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _products = database.GetCollection<Product>("products");
        }

        // ✅ méthode manquante
        public async Task<List<Product>> GetAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        // ✅ méthode manquante
        public async Task<Product> CreateAsync(Product product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }
    }
}