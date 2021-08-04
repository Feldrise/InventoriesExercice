using InventoriesExercise.API.Entities;
using InventoriesExercise.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Services.Interfaces
{
    public interface IItemsService
    {
        Task<List<Item>> GetAllAsync(string userId, string inventoryId);
        Task<Item> GetAsync(string userId, string id);

        Task<string> CreateAsync(string userId, ItemModel itemModel);
        Task UpdateAsync(string userId, string id, ItemModel itemModel);

        Task DeleteAsync(string userId, string id);
    }
}
