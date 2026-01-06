using cancrops.src.implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cancrops.src.utility
{
    /// <summary>
    /// Registry for all configured plant types that can be used with the crop breeding system.
    /// Plants are loaded from JSON configuration files at startup.
    /// </summary>
    public class AgriPlants
    {
        private Dictionary<string, AgriPlant> plants;

        public AgriPlants()
        {
            plants = new Dictionary<string, AgriPlant>();
        }
        
        /// <summary>
        /// Check if a plant with the given id exists in the registry
        /// </summary>
        /// <param name="id">Full plant id in format "domain:name" (e.g., "game:carrot")</param>
        public bool hasPlant(string id)
        {
            return plants.ContainsKey(id);
        }

        /// <summary>
        /// Register a new plant in the system
        /// </summary>
        /// <param name="plant">The plant configuration to add</param>
        /// <returns>True if successfully added, false if already exists</returns>
        public bool addPlant(AgriPlant plant)
        {
            return plants.TryAdd(plant.Domain + ":" + plant.Id, plant);
        }

        /// <summary>
        /// Retrieve a plant configuration by its id
        /// </summary>
        /// <param name="id">Full plant id in format "domain:name" (e.g., "game:carrot")</param>
        /// <returns>The plant configuration, or null if not found</returns>
        public AgriPlant getPlant(string id)
        {
            if (plants.TryGetValue(id, out AgriPlant val))
            {
                return val;
            }
            return null;
        }
    }
}
