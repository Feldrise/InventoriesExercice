using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Models.Users
{
    public class LoginModel
    {
        /// <summary>
        /// The user's email
        /// </summary>
        /// <example>admin@feldrise.com</example>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
        /// <example>MySyperSecurePassword</example>
        [Required]
        public string Password { get; set; }
    }
}
