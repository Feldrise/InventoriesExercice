using InventoriesExercice.API.Entities;
using InventoriesExercice.API.Models.Users;
using InventoriesExercice.API.Services.Interfaces;
using InventoriesExercice.API.Settings.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InventoriesExercice.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMongoCollection<User> _users;

        private readonly IInventoriesExerciceSettings _inventoriesExerciceSettings;

        public AuthenticationService(IMongoSettings mongoSettings, IInventoriesExerciceSettings inventoriesExerciceSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _users = database.GetCollection<User>(mongoSettings.UsersCollectionName);

            _inventoriesExerciceSettings = inventoriesExerciceSettings;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            // We don't send exception here, the request is only
            // valid for non null values
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var user = await (await _users.FindAsync(dbUser =>
                dbUser.Email == email.ToLower()
            )).FirstOrDefaultAsync();

            if (user == null) { return null; }
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) { return null; }

            // Since the authentication is successful, now we can
            // generate the token
            user.Token = TokenForUser(user);

            return user;
        }

        public async Task<string> RegisterAsync(RegisterModel registerModel)
        {
            // We need some basic checks
            if (string.IsNullOrWhiteSpace(registerModel.Email)) { throw new Exception("An email is required for the registration"); }
            if (UserExist(registerModel.Email)) { throw new Exception("The user already exist"); }

            // Password stuff, to ensure we never have clear password stored
            CreatePasswordHash(registerModel.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Now we register the user in the database
            User dbUser = new()
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,

                Email = registerModel.Email.ToLower(),

                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = passwordSalt,
            };

            await _users.InsertOneAsync(dbUser);

            return dbUser.Id;
        }

        private bool UserExist(string email)
        {
            return _users.AsQueryable().Any(dbUser =>
                dbUser.Email == email.ToLower()
            );
        }

        // Password related functions
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password)) { throw new Exception("The password must not be null or empty"); }

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, string storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password)) { throw new Exception("The password must not be null or empy"); }
            if (storedSalt.Length != 128) { throw new Exception("Invalid length of password salt (128 bytes expected)"); }

            byte[] storedHashBytes = Convert.FromBase64String(storedHash);

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; ++i)
                {
                    if (computedHash[i] != storedHashBytes[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Generate the token for the user
        private string TokenForUser(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_inventoriesExerciceSettings.ApiSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
