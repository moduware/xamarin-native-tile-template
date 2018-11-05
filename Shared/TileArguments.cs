using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTemplate.Shared
{
    /// <summary>
    /// Arguments that the tile receives from Moduware dashboard
    /// </summary>
    public class TileArguments
    {
        /// <summary>
        /// Target module uuid
        /// </summary>
        [JsonProperty("uuid")]
        public string Uuid;

        /// <summary>
        /// Target module slot
        /// </summary>
        [JsonProperty("slot")]
        public string Slot;

        /// <summary>
        /// Target module type
        /// </summary>
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("0")]
        public string First => Uuid;

        [JsonProperty("1")]
        public string Second => Slot;

        [JsonProperty("2")]
        public string Third => Type;

        public TileArguments() { } // for deserialization

        public TileArguments(string uuid, string slot, string type)
        {
            Uuid = uuid;
            Slot = slot;
            Type = type;
        }
    }
}
