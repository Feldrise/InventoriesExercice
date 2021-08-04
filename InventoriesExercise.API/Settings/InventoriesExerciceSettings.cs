using InventoriesExercise.API.Settings.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoriesExercise.API.Settings
{
    public class InventoriesExerciseSettings : IInventoriesExerciseSettings
    {
        public string ApiSecret { get; set; }
    }
}
