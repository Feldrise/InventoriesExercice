using InventoriesExercise.API.Entities;
using InventoriesExercise.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Services.Interfaces
{
    public interface IInventoriesService
    {
        Task<List<Inventory>> GetAllAsync(string userId);
        Task<Inventory> GetAsync(string userId, string slug);

        Task<string> CreateAsync(string userId, InventoryModel inventoryModel);
        Task UpdateAsync(string userId, string id, InventoryModel inventoryModel);

        Task DeleteAsync(string userId, string id);
    }
}
