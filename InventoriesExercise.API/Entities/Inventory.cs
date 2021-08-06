using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Entities
{
    public class Inventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The id of the user who created the inventory
        /// </summary>
        /// <example>61099647773fb8bad8df5a8b</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; }

        /// <summary>
        /// The inventory slug, must be unique
        /// </summary>
        /// <example>cuddly_toys</example>
        public string Slug { get; set; }

        /// <summary>
        /// The inventory name
        /// </summary>
        /// <example>Cuddly Toys</example>
        public string Name { get; set; }
        /// <summary>
        /// The inventory description 
        /// </summary>
        /// <example>A stock of nice cuddly toys</example>
        public string Description { get; set; }
    }
}
