using server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace server.Services
{
  public class MongoDBService
  {
    private readonly IMongoCollection<Product> _productCollection;
    // private readonly IMongoCollection<Pet> _petCollection;

    public MongoDBService(IOptions<MongoDBConfig> mongoDBConfigs)
    {
      try
      {
        MongoClient client = new MongoClient(mongoDBConfigs.Value.MongoURI);
        IMongoDatabase database = client.GetDatabase(mongoDBConfigs.Value.DbName);

        // Initialize the collections
        _productCollection = database.GetCollection<Product>(mongoDBConfigs.Value.MongoProductCollection);
        // _petCollection = database.GetCollection<Pet>(mongoDBConfigs.Value.MongoPetCollection);

        // Log a message when connected
        Console.WriteLine("Successfully connected to MongoDB");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"MongoDB connection failed: {ex.Message}");
        throw; // Rethrow the exception to handle it upstream
      }
    }

    // Product methods
    public async Task<List<Product>> GetProductsAsync()
    {
      return await _productCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<Product?> GetProductAsync(string id) =>
        await _productCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateProductAsync(Product product)
    {
      await _productCollection.InsertOneAsync(product);
    }

    public async Task UpdateProductAsync(string id, Product updatedProduct) =>
        await _productCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct);

    public async Task<bool> DeleteProductAsync(string id)
    {
      FilterDefinition<Product> filter = Builders<Product>.Filter.Eq("Id", id);
      var result = await _productCollection.DeleteOneAsync(filter);
      return result.DeletedCount > 0;
    }

    public async Task<bool> ChangeProductStatusAsync(string id, bool isActive)
    {
      var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
      var update = Builders<Product>.Update.Set(p => p.IsActive, isActive);

      var result = await _productCollection.UpdateOneAsync(filter, update);
      return result.ModifiedCount > 0;
    }

    //Inventory methods
    public async Task<bool> DeductAvailableStock(string id, int quantity)
    {
      var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
      var updatedAvailableStock = quantity;
      var update = Builders<Product>.Update.Set(p => p.StockLevel, updatedAvailableStock);

      var result = await _productCollection.UpdateOneAsync(filter, update);
      return result.ModifiedCount > 0;
    }

    // Pet methods
    // public async Task<List<Pet>> GetPetsAsync()
    // {
    //   return await _petCollection.Find(new BsonDocument()).ToListAsync();
    // }

    // public async Task<Pet?> GetPetAsync(string id) =>
    //     await _petCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    // public async Task CreatePetAsync(Pet pet)
    // {
    //   await _petCollection.InsertOneAsync(pet);
    // }

    // public async Task UpdatePetAsync(string id, Pet updatedPet) =>
    //     await _petCollection.ReplaceOneAsync(x => x.Id == id, updatedPet);

    // public async Task DeletePetAsync(string id)
    // {
    //   FilterDefinition<Pet> filter = Builders<Pet>.Filter.Eq("Id", id);
    //   await _petCollection.DeleteOneAsync(filter);
    // }
  }
}
