using InventoriesExercice.API.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercice.API.Settings
{
    public class MongoSettings : IMongoSettings
    {
        public string UsersCollectionName { get; set; }
        public string InventoriesCollectionName { get; set; }
        public string ItemsCollectionName { get; set; }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
