using InventoriesExercise.API.Entities;
using InventoriesExercise.API.Models;
using InventoriesExercise.API.Services.Interfaces;
using InventoriesExercise.API.Settings.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Services
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

        public async Task<Inventory> GetAsync(string userId, string slug)
        {
            Inventory inventory = await GetFromSlugAsync(slug);

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
            string slug = GenerateSlug(inventoryModel.Name);

            if (await DoesSlugExist(slug))
            {
                throw new Exception("The slug already exist. You should change the inventory name");
            }

            Inventory inventory = new()
            {
                OwnerId = userId,
                Slug = slug,
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

            string slug = GenerateSlug(inventoryModel.Name);

            if (await DoesSlugExist(slug, id))
            {
                throw new Exception("The slug already exist. You should change the inventory name");
            }

            // Then we can update
            var update = Builders<Inventory>.Update
                .Set(dbInventory => dbInventory.Slug, slug)
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

        private async Task<Inventory> GetFromSlugAsync(string slug)
        {
            var dbInventory = await _inventories.FindAsync(
                dbInventory => dbInventory.Slug == slug
            );

            return await dbInventory.FirstOrDefaultAsync();
        }

        private async Task<bool> CanEditAsync(string userId, string id)
        {
            Inventory inventory = await GetFromIdAsync(id);

            return inventory != null && inventory.OwnerId == userId;
        }

        private async Task<bool> DoesSlugExist(string slug, string currentInventoyId = null)
        {
            var dbInventory = await _inventories.FindAsync(
                dbInventory => dbInventory.Slug == slug
            );

            Inventory inventory = await dbInventory.FirstOrDefaultAsync();

            return inventory != null &&
                   inventory.Id != currentInventoyId;
        }

        /// See: http://www.siao2.com/2007/05/14/2629747.aspx
        private static string RemoveDiacritics(string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        private static string GenerateSlug(string phrase)
        {
            string str = RemoveDiacritics(phrase).ToLower();

            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }
    }
}
