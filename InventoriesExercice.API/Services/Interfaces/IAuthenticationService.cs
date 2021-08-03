using InventoriesExercice.API.Entities;
using InventoriesExercice.API.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercice.API.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> LoginAsync(string email, string password);
        Task<string> RegisterAsync(RegisterModel registerModel);
    }
}
