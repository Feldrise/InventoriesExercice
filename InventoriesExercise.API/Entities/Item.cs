using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Entities
{
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The id of the inventory the item is in
        /// </summary>
        /// <example>61099647773fb8bad8df5a8b</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string InventoryId { get; set; }

        /// <summary>
        /// The item name
        /// </summary>
        /// <example>Teddy Bear</example>
        public string Name { get; set; }
        /// <summary>
        /// The item description 
        /// </summary>
        /// <example>Simple teddy bear, fluffy and lovely for all children</example>
        public string Description { get; set; }
        /// <summary>
        /// The number of items in stock
        /// </summary>
        /// <example>3</example>
        public int Quantity { get; set; }
    }
}
