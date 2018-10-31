using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTemplate.Shared
{
    public class TileArguments
    {
        [JsonProperty("uuid")]
        public string Uuid;

        [JsonProperty("slot")]
        public string Slot;

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
