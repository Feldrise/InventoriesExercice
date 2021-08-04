using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Settings.Interfaces
{
    public interface IMongoSettings
    {
        string UsersCollectionName { get; set; }
        string InventoriesCollectionName { get; set; }
        string ItemsCollectionName { get; set; }

        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
