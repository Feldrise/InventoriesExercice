using InventoriesExercice.API.Entities;
using InventoriesExercice.API.Models;
using InventoriesExercice.API.Services.Interfaces;
using InventoriesExercice.API.Settings.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercice.API.Services
{
    public class InventoriesService : IInventoriesService
    {
        private readonly IMongoCollection<Inventory> _inventories;

        public InventoriesService(IMongoSettings mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _inventories = database.GetCollection<Inventory>(mongoSettings.InventoriesCollectionName);
        }

        public async Task<List<Inventory>> GetAllAsync(string userId)
        {
            var dbInventories = await _inventories.FindAsync(
                dbInventory => dbInventory.OwnerId == userId
            );

            return await dbInventories.ToListAsync();
        }

        public async Task<Inventory> GetAsync(string userId, string id)
        {
            Inventory inventory = await GetFromIdAsync(id);

            if (inventory == null)
            {
                return null;
            }

            if (inventory.OwnerId != userId)
            {
                throw new UnauthorizedAccessException("You are not the owner of this inventory");
            }

            return inventory;
        }

        public async Task<string> CreateAsync(string userId, InventoryModel inventoryModel)
        {
            Inventory inventory = new()
            {
                OwnerId = userId,
                Name = inventoryModel.Name,
                Description = inventoryModel.Description
            };

            await _inventories.InsertOneAsync(inventory);

            return inventory.Id;
        }

        public async Task UpdateAsync(string userId, string id, InventoryModel inventoryModel)
        {
            // First, we need to make sure the user have the 
            // right to update this inventory
            bool canEdit = await CanEditAsync(userId, id);

            if (!canEdit)
            {
                throw new UnauthorizedAccessException("You don't have the rights to update this inventory");
            }

            // Then we can update
            var update = Builders<Inventory>.Update
                .Set(dbInventory => dbInventory.Name, inventoryModel.Name)
                .Set(dbInventory => dbInventory.Description, inventoryModel.Description);

            await _inventories.UpdateOneAsync(
                dbInventory => dbInventory.Id == id,
                update
            );
        }

        public async Task DeleteAsync(string userId, string id)
        {
            // First, we need to make sure the user have the 
            // right to update this inventory
            bool canEdit = await CanEditAsync(userId, id);

            // The we can delete 
            if (!canEdit)
            {
                throw new UnauthorizedAccessException("You don't have the rights to delete this inventory");
            }

            await _inventories.DeleteOneAsync(
                dbInventory => dbInventory.Id == id
            );
        }

        private async Task<Inventory> GetFromIdAsync(string id)
        {
            var dbInventory = await _inventories.FindAsync(
                dbInventory => dbInventory.Id == id
            );

            return await dbInventory.FirstOrDefaultAsync();
        }

        private async Task<bool> CanEditAsync(string userId, string id)
        {
            Inventory inventory = await GetFromIdAsync(id);

            return inventory != null && inventory.OwnerId == userId;
        }
    }
}
