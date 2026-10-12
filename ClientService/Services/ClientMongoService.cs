using MongoDB.Driver;
using ClientService.Models;
using ClientService.Settings;

namespace ClientService.Services
{
    public class ClientMongoService
    {
        private readonly IMongoCollection<Client> _clients;

        public ClientMongoService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _clients = database.GetCollection<Client>("clients");
        }

        public async Task<List<Client>> GetAsync()
        {
            return await _clients.Find(_ => true).ToListAsync();
        }

        public async Task<Client?> GetAsync(string id)
        {
            return await _clients.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Client client)
        {
            await _clients.InsertOneAsync(client);
        }

        public async Task UpdateAsync(string id, Client client)
        {
            await _clients.ReplaceOneAsync(c => c.Id == id, client);
        }

        public async Task DeleteAsync(string id)
        {
            await _clients.DeleteOneAsync(c => c.Id == id);
        }
    }
}