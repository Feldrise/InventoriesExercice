using InventoriesExercise.API.Entities;
using InventoriesExercise.API.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> LoginAsync(string email, string password);
        Task<string> RegisterAsync(RegisterModel registerModel);
    }
}
