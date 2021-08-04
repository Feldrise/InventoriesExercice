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
    public class ItemsService : IItemsService
    {
        private readonly IMongoCollection<Item> _items;
        private readonly IMongoCollection<Inventory> _inventories;

        public ItemsService(IMongoSettings mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _items = database.GetCollection<Item>(mongoSettings.ItemsCollectionName);
            _inventories = database.GetCollection<Inventory>(mongoSettings.InventoriesCollectionName);
        }

        public async Task<List<Item>> GetAllAsync(string userId, string inventoryId)
        {
            if (!(await IsInventoryAccessible(userId, inventoryId)))
            {
                throw new UnauthorizedAccessException();
            }

            var dbItems = await _items.FindAsync(
                dbItem => dbItem.InventoryId== inventoryId
            );

            return await dbItems.ToListAsync();
        }

        public async Task<Item> GetAsync(string userId, string id)
        {
            Item item = await GetFromIdAsync(id);

            if (item == null)
            {
                return null;
            }

            if (!(await IsInventoryAccessible(userId, item.InventoryId)))
            {
                throw new UnauthorizedAccessException();
            }

            return item;
        }

        public async Task<string> CreateAsync(string userId, ItemModel itemModel)
        {
            // We make sure the inventory of the item exists
            if ((await GetInventoryFromId(itemModel.InventoryId) == null))
            {
                throw new ArgumentException("You are trying to create an item to an unknown inventory", nameof(itemModel));
            }

            // And that we can add items to it
            if (!(await IsInventoryAccessible(userId, itemModel.InventoryId))) 
            {
                throw new UnauthorizedAccessException();
            }

            Item item = new()
            {
                InventoryId = itemModel.InventoryId,
                Name = itemModel.Name,
                Description = itemModel.Description,
                Quantity = itemModel.Quantity
            };

            await _items.InsertOneAsync(item);

            return item.Id;
        }

        public async Task UpdateAsync(string userId, string id, ItemModel itemModel)
        {
            // First, we need to make sure the user have the 
            // right to update this item
            bool canEdit = await IsInventoryAccessible(userId, itemModel.InventoryId); 

            if (!canEdit)
            {
                throw new UnauthorizedAccessException();
            }

            // Then we can update
            var update = Builders<Item>.Update
                .Set(dbItem => dbItem.Name, itemModel.Name)
                .Set(dbItem => dbItem.Description, itemModel.Description)
                .Set(dbItem => dbItem.Quantity, itemModel.Quantity);

            await _items.UpdateOneAsync(
                dbItem => dbItem.Id == id,
                update
            );
        }

        public async Task DeleteAsync(string userId, string id)
        {
            // First, we need to make sure the user have the 
            // right to update this inventory
            Item item = await GetFromIdAsync(id);

            bool canEdit = item != null && await IsInventoryAccessible(userId, item.InventoryId);

            // The we can delete 
            if (!canEdit)
            {
                throw new UnauthorizedAccessException();
            }

            await _items.DeleteOneAsync(
                dbItem => dbItem.Id == id
            );
        }

        private async Task<Item> GetFromIdAsync(string id)
        {
            var dbItem = await _items.FindAsync(
                dbItem => dbItem.Id == id
            );

            return await dbItem.FirstOrDefaultAsync();
        }

        private async Task<Inventory> GetInventoryFromId(string inventoryId)
        {
            var dbInventory = await _inventories.FindAsync(
                dbInventory => dbInventory.Id == inventoryId
            );

            return await dbInventory.FirstOrDefaultAsync();
        }

        private async Task<bool> IsInventoryAccessible(string userId, string inventoryId)
        {
            Inventory inventory = await GetInventoryFromId(inventoryId);

            return (inventory != null && inventory.OwnerId == userId);
        }

    }
}
