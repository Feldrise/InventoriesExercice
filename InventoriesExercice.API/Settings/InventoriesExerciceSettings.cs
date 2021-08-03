using InventoriesExercice.API.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercice.API.Settings
{
    public class InventoriesExerciceSettings : IInventoriesExerciceSettings
    {
        public string ApiSecret { get; set; }
    }
}
