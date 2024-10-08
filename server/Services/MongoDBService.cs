using server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace server.Services
{
  public class MongoDBService
  {
    private readonly IMongoCollection<Product> _productCollection;
    private readonly IMongoCollection<Comments> _commentCollection;
    private readonly IMongoCollection<Admin> _adminCollection;
    private readonly IMongoCollection<Order> _orderCollection;

    private readonly IMongoCollection<Notification> _notificationCollection;

    private readonly IMongoCollection<CSR> _csrCollection;

    private readonly IMongoCollection<Users> _customerCollection;

    private readonly IMongoCollection<Vendor> _vendorCollection;


    public MongoDBService(IOptions<MongoDBConfig> mongoDBConfigs)
    {
      try
      {
        MongoClient client = new MongoClient(mongoDBConfigs.Value.MongoURI);
        IMongoDatabase database = client.GetDatabase(mongoDBConfigs.Value.DbName);

        // Initialize the collections
        _productCollection = database.GetCollection<Product>(mongoDBConfigs.Value.MongoProductCollection);
        _commentCollection = database.GetCollection<Comments>(mongoDBConfigs.Value.MongoCommentCollection);
        _adminCollection = database.GetCollection<Admin>(mongoDBConfigs.Value.MongoAdminCollection);
        _orderCollection = database.GetCollection<Order>(mongoDBConfigs.Value.MongoOrderCollection);
        _notificationCollection = database.GetCollection<Notification>(mongoDBConfigs.Value.MongoNotificationCollection);
        _csrCollection = database.GetCollection<CSR>(mongoDBConfigs.Value.MongoCSRCollection);
        _vendorCollection = database.GetCollection<Vendor>(mongoDBConfigs.Value.MongoVendorCollection);
        _customerCollection = database.GetCollection<Users>(mongoDBConfigs.Value.MongoCustomerCollection);

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

    public async Task<List<Product>> GetProductsCustomerAsync()
    {
      var filterBuilder = Builders<Product>.Filter;
      var filterActiveCategory = filterBuilder.Eq(p => p.IsCategoryActive, true);
      var filterActiveProducts = filterBuilder.Eq(p => p.IsActive, true);
      var combinedFilter = filterBuilder.And(filterActiveCategory, filterActiveProducts);
      return await _productCollection.Find(combinedFilter).ToListAsync();
    }

    public async Task<List<string>> GetDistinctCategoriesAsync()
    {
      var filterBuilder = Builders<Product>.Filter;
      var filterActiveCategory = filterBuilder.Eq(p => p.IsCategoryActive, true);
      return await _productCollection.Distinct<string>("Category", filterActiveCategory).ToListAsync();
    }


    public async Task<List<Product>> GetProductsVenderAsync(string venderId)
    {
      var filter = Builders<Product>.Filter.Eq(p => p.VenderId, venderId);
      return await _productCollection.Find(filter).ToListAsync();
    }

    public async Task<List<Product>> SearchProduct(string productName)
    {
      var filterBuilder = Builders<Product>.Filter;
      var nameFilter = filterBuilder.Regex("ProductName", new BsonRegularExpression($"^{productName}", "i"));
      var activeFilter = filterBuilder.Eq("IsActive", true);
      var filterActiveCategory = filterBuilder.Eq(p => p.IsCategoryActive, true);
      var combinedFilter = filterBuilder.And(nameFilter, activeFilter, filterActiveCategory);
      return await _productCollection.Find(combinedFilter).ToListAsync();
    }

    public async Task<List<Product>> GetProductsCategoryBasedAsync(string category)
    {
      var filterBuilder = Builders<Product>.Filter;
      var categoryFilter = Builders<Product>.Filter.Eq(p => p.Category, category);
      var activeFilter = filterBuilder.Eq("IsActive", true);
      var filterActiveCategory = filterBuilder.Eq(p => p.IsCategoryActive, true);
      var combinedFilter = filterBuilder.And(categoryFilter, activeFilter, filterActiveCategory);
      return await _productCollection.Find(combinedFilter).ToListAsync();
    }

    public async Task<List<Product>> GetAllProductsBasedOnCategoryAsync(string category)
    {
      var filterBuilder = Builders<Product>.Filter;
      var categoryFilter = Builders<Product>.Filter.Eq(p => p.Category, category);
      var combinedFilter = filterBuilder.And(categoryFilter);
      return await _productCollection.Find(combinedFilter).ToListAsync();
    }

    public async Task<Product?> GetProductAsync(string id) =>
        await _productCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateProductAsync(Product product)
    {
      await _productCollection.InsertOneAsync(product);
    }

    public async Task UpdateProductAsync(string id, Product updatedProduct) =>
        await _productCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct);

    public async Task<UpdateResult> UpdateCategoryStatusAsync(string category, bool newStatus)
    {
      var filter = Builders<Product>.Filter.Eq(p => p.Category, category);

      var update = Builders<Product>.Update.Set(p => p.IsCategoryActive, newStatus);

      return await _productCollection.UpdateManyAsync(filter, update);
    }

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

    //Comment Methods
    public async Task CreateCommentAndRate(Comments comment)
    {
      await _commentCollection.InsertOneAsync(comment);
    }

    public async Task<List<Comments>> GetCommentsAsync()
    {
      return await _commentCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<Comments?> GetCommentAsync(string id) =>
        await _commentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Comments>> GetCustomerSpecificCommentsAsync(string customerId)
    {
      return await _commentCollection.Find(x => x.CustomerId == customerId).ToListAsync();
    }

    public async Task<List<Comments>> GetVenderSpecificCommentsAsync(string venderId)
    {
      return await _commentCollection.Find(x => x.VendorId == venderId).ToListAsync();
    }

    public async Task UpdateCommentAsync(string id, Comments updatedComment) =>
        await _commentCollection.ReplaceOneAsync(x => x.Id == id, updatedComment);


    public async Task<bool> DeleteCommentAsync(string id)
    {
      FilterDefinition<Comments> filter = Builders<Comments>.Filter.Eq("Id", id);
      var result = await _commentCollection.DeleteOneAsync(filter);
      return result.DeletedCount > 0;
    }

    //User Management Methods

    // Create admin
    public async Task<Admin> CreateAdminAsync(Admin admin)
    {
      if (admin == null)
      {
        throw new ArgumentNullException(nameof(admin), "Admin cannot be null.");
      }

      await _adminCollection.InsertOneAsync(admin);
      var adminWithoutPassword = new Admin
      {
        Id = admin.Id,
        Username = admin.Username,
        Email = admin.Email,
        AdminsCreated = admin.AdminsCreated,
        CSRCreated = admin.CSRCreated
      };

      return adminWithoutPassword;
    }


    //Get admin by ID
    public async Task<Admin?> GetAdminByIdAsync(string id) =>
        await _adminCollection.Find(x => x.Id == id).FirstOrDefaultAsync();


    //Get admin by email
    public async Task<Admin?> GetAdminByEmailAsync(string email) =>
        await _adminCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

    // Update admin by ID (username, email, password)
    public async Task UpdateAdminAsync(string adminId, Admin updatedAdmin)
    {
      var filter = Builders<Admin>.Filter.Eq(a => a.Id, adminId);

      var update = Builders<Admin>.Update
          .Set(a => a.Username, updatedAdmin.Username)
          .Set(a => a.Email, updatedAdmin.Email)
          .Set(a => a.Password, updatedAdmin.Password);

      var updateResult = await _adminCollection.UpdateOneAsync(filter, update);

      if (updateResult.MatchedCount == 0)
      {
        throw new Exception($"Admin with ID {adminId} not found.");
      }
    }


    // Update CSR by ID
    public async Task UpdateCSRByIDAsync(string csrId, CSR updatedCSR)
    {
      var filter = Builders<CSR>.Filter.Eq(a => a.Id, csrId);
      var updateResult = await _csrCollection.ReplaceOneAsync(filter, updatedCSR);

      if (updateResult.MatchedCount == 0)
      {
        throw new Exception($"CSR with ID {csrId} not found.");
      }
    }

    // Update Vendor by ID
    public async Task UpdateVendorAsync(string vendorId, Vendor updatedVendor)
    {
      var filter = Builders<Vendor>.Filter.Eq(a => a.Id, vendorId);
      var updateResult = await _vendorCollection.ReplaceOneAsync(filter, updatedVendor);

      if (updateResult.MatchedCount == 0)
      {
        throw new Exception($"Admin with ID {vendorId} not found.");
      }
    }

    // Update Customer by ID
    public async Task UpdateCustomerAsync(string customerId, Users updatedCustomer)
    {
      var filter = Builders<Users>.Filter.Eq(a => a.Id, customerId);
      var updateResult = await _customerCollection.ReplaceOneAsync(filter, updatedCustomer);

      if (updateResult.MatchedCount == 0)
      {
        throw new Exception($"Customer with ID {customerId} not found.");
      }
    }

    //Get CSR by Email
    public async Task<CSR?> GetCSRByEmailAsync(string email) =>
        await _csrCollection.Find(x => x.Email == email).FirstOrDefaultAsync();


    //Create a new CSR
    public async Task<CSR> CreateCSRAsync(CSR csr)
    {
      await _csrCollection.InsertOneAsync(csr);
      return csr;
    }

    //Get vendor by email
    public async Task<Vendor?> GetVendorByEmailAsync(string email) =>
        await _vendorCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

    //Create a new Vendor
    public async Task<Vendor> CreateVendorAsync(Vendor vendor)
    {
      await _vendorCollection.InsertOneAsync(vendor);
      return vendor;
    }

    //Sign up for new customer
    public async Task<Users> CreateCustomerAsync(Users customer)
    {
      await _customerCollection.InsertOneAsync(customer);
      return customer;
    }

    //Get customer by email
    public async Task<Users?> GetCustomerByEmailAsync(string email) =>
        await _customerCollection.Find(x => x.Email == email).FirstOrDefaultAsync();



    // Update User by ID based on role
    public async Task<dynamic> UpdateUserAsync(string userId, string role, dynamic updatedUser)
    {

      // Role-based logic to update user
      switch (role.ToLower())
      {
        case "admin":
          var admin = await GetAdminByIdAsync(userId);
          if (admin == null)
          {
            throw new Exception($"Admin with ID {userId} not found.");
          }
          await UpdateAdminAsync(userId, updatedUser);
          break;

        case "csr":
          var csr = await GetCSRByIdAsync(userId);
          if (csr == null)
          {
            throw new Exception($"CSR with ID {userId} not found.");
          }
          await UpdateCSRByIDAsync(userId, updatedUser);
          break;

        case "vendor":
          var vendor = await GetVendorByIdAsync(userId);
          if (vendor == null)
          {
            throw new Exception($"Vendor with ID {userId} not found.");
          }
          await UpdateVendorAsync(userId, updatedUser);
          break;

        case "customer":
          var customer = await GetCustomerByIdAsync(userId);
          if (customer == null)
          {
            throw new Exception($"Customer with ID {userId} not found.");
          }
          await UpdateCustomerAsync(userId, updatedUser);
          break;

        default:
          throw new Exception("Invalid role provided.");
      }

      // Return the updated user details if necessary (optional, remove if not needed)
      return updatedUser;
    }

    // Get CSR by ID
    public async Task<CSR?> GetCSRByIdAsync(string id) =>
      await _csrCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    // Get Vendor by ID
    public async Task<Vendor?> GetVendorByIdAsync(string id) =>
      await _vendorCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    // Get Customer by ID
    public async Task<Users?> GetCustomerByIdAsync(string id) =>
      await _customerCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    //Get all Vendors
    public async Task<List<Vendor>> GetVendorsAsync()
    {
      var vendors = await _vendorCollection.Find(new BsonDocument())
          .Project<Vendor>(Builders<Vendor>.Projection
          .Exclude(vendor => vendor.Password))
          .ToListAsync();

      return vendors;
    }

    //Get all Customers
    public async Task<List<Users>> GetAllCustomersAsync()
    {
      var customers = await _customerCollection.Find(new BsonDocument())
          .ToListAsync();

      return customers;
    }

    ///Get all csr
    public async Task<List<CSR>> GetAllCSRsAsync()
    {
      var csrs = await _csrCollection.Find(new BsonDocument())
          .ToListAsync();

      return csrs;
    }

    //Get all Admins
    public async Task<List<Admin>> GetAllAdminsAsync()
    {
      var admins = await _adminCollection.Find(new BsonDocument())
          .ToListAsync();

      return admins;
    }

    //Update Customer 
    public async Task UpdateCustomer(string customerId, Users updatedCustomer)
    {
      var filter = Builders<Users>.Filter.Eq(a => a.Id, customerId);
      var updateResult = await _customerCollection.ReplaceOneAsync(filter, updatedCustomer);

      if (updateResult.MatchedCount == 0)
      {
        throw new Exception($"Customer with ID {customerId} not found.");
      }
    }

    //Update CSR
    public async Task UpdateCSR(string csrId, CSR updatedCSR)
    {
      var filter = Builders<CSR>.Filter.Eq(a => a.Id, csrId);
      var updateResult = await _csrCollection.ReplaceOneAsync(filter, updatedCSR);

      if (updateResult.MatchedCount == 0)
      {
        throw new Exception($"CSR with ID {csrId} not found.");
      }
    }

    //Get all approved customers
    public async Task<List<Users>> GetApprovedCustomersAsync()
    {
      return await _customerCollection.Find(x => x.ApprovalStatus == true)
          .Project<Users>(Builders<Users>.Projection
          .Exclude(customer => customer.Password))
          .ToListAsync();

    }
    //Get all unapproved customers
    public async Task<List<Users>> GetUnapprovedCustomersAsync()
    {
      return await _customerCollection.Find(x => x.ApprovalStatus == false)
          .Project<Users>(Builders<Users>.Projection
          .Exclude(customer => customer.Password))
          .ToListAsync();
    }

    //Get all approved customers by ID based on Approved By
    public async Task<List<Users>> GetApprovedCustomersByIdAsync(string adminId)
    {
      return await _customerCollection.Find(x => x.ApprovedBy == adminId && x.ApprovalStatus == true)
          .Project<Users>(Builders<Users>.Projection
          .Exclude(customer => customer.Password))
          .ToListAsync();
    }

    //Get All deactivated customers
    public async Task<List<Users>> GetDeactivatedCustomersAsync()
    {
      return await _customerCollection.Find(x => x.Deactivated == true).ToListAsync();
    }

    //Update User Password
    public async Task UpdateUserPasswordAsync(string userId, string newPassword, string role)
    {
      switch (role.ToLower())
      {
        case "admin":
          var admin = await GetAdminByIdAsync(userId);
          if (admin == null)
          {
            throw new Exception($"Admin with ID {userId} not found.");
          }
          admin.Password = newPassword;
          await UpdateAdminAsync(userId, admin);
          break;

        case "csr":
          var csr = await GetCSRByIdAsync(userId);
          if (csr == null)
          {
            throw new Exception($"CSR with ID {userId} not found.");
          }
          csr.Password = newPassword;
          await UpdateCSRByIDAsync(userId, csr);
          break;

        case "vendor":
          var vendor = await GetVendorByIdAsync(userId);
          if (vendor == null)
          {
            throw new Exception($"Vendor with ID {userId} not found.");
          }
          vendor.Password = newPassword;
          await UpdateVendorAsync(userId, vendor);
          break;

        case "customer":
          var customer = await GetCustomerByIdAsync(userId);
          if (customer == null)
          {
            throw new Exception($"Customer with ID {userId} not found.");
          }
          customer.Password = newPassword;
          await UpdateCustomerAsync(userId, customer);
          break;

        default:
          throw new Exception("Invalid role provided.");
      }
    }

    private async Task UpdateCSRAsync(string userId, CSR csr)
    {
      throw new NotImplementedException();
    }


    //Add a new order
    public async Task<Order> CreateOrder(Order order)
    {
      await _orderCollection.InsertOneAsync(order);
      return order;
    }

    //Get order by ID
    public async Task<Order?> GetOrderByIdAsync(string id) =>
        await _orderCollection.Find(x => x.Id == id).FirstOrDefaultAsync();


    //Get order by OrderId
    public async Task<Order?> GetOrderByOrderIdAsync(string orderId) =>
        await _orderCollection.Find(x => x.OrderId == orderId).FirstOrDefaultAsync();

    //Update Order
    public async Task UpdateOrder(Order order)
    {
      var filter = Builders<Order>.Filter.Eq(a => a.Id, order.Id);
      var updateResult = await _orderCollection.ReplaceOneAsync(filter, order);

      if (updateResult.MatchedCount == 0)
      {
        throw new Exception($"Order with ID {order.Id} not found.");
      }
    }

    //Get All Requests to cancel orders
    public async Task<List<Order>> GetRequestToCancelOrdersAsync()
    {
      return await _orderCollection.Find(x => x.RequestToCancel == true).ToListAsync();
    }

    //Get All Cancelled Orders where Request to cancel is false and Cancelled is true
    public async Task<List<Order>> GetCancelledOrdersAsync()
    {
      return await _orderCollection.Find(x => x.RequestToCancel == false && x.Cancelled == true).ToListAsync();
    }

    //Get all orders
    public async Task<List<Order>> GetAllOrdersAsync()
    {
      return await _orderCollection.Find(new BsonDocument()).ToListAsync();
    }

    //Get order by userId
    public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
    {
      return await _orderCollection.Find(x => x.UserId == userId).ToListAsync();
    }

    //Get orders cancelled by csr by CSR ID
    public async Task<List<Order>> GetOrdersCancelledByCSRAsync(string csrId)
    {
      return await _orderCollection.Find(x => x.CancelledBy == csrId).ToListAsync();
    }

    //Add Notification
    public async Task<Notification> CreateNotification(Notification notification)
    {
      await _notificationCollection.InsertOneAsync(notification);
      return notification;
    }

    //Get Notification by ID
    public async Task<Notification?> GetNotificationByIdAsync(string id) =>
        await _notificationCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Notification>> GetNotificationsByUserID(string id) =>
    await _notificationCollection.Find(x => x.UserId == id).ToListAsync();

    //Delete Notification
    public async Task DeleteNotificationAsync(string notificationId)
    {
      FilterDefinition<Notification> filter = Builders<Notification>.Filter.Eq("Id", notificationId);
      await _notificationCollection.DeleteOneAsync(filter);
    }
  }
}