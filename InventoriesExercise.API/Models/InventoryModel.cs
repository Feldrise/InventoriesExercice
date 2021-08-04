using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Models
{
    public class InventoryModel
    {
        /// <summary>
        /// The inventory name
        /// </summary>
        /// <example>Cuddly Toys</example>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// The inventory description 
        /// </summary>
        /// <example>A stock of nice cuddly toys</example>
        [Required]
        public string Description { get; set; }
    }
}
