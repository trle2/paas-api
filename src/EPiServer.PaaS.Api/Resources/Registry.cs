using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EPiServer.PaaS.Api.Resources
{
    public class RegistryList
    {
        [JsonPropertyName("value")]
        public List<Registry> Registries { get; set; }
    }

    public class Registry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("properties")]
        public RegistryProperties Properties { get; set; }
    }

    public class RegistryProperties
    {

        [JsonPropertyName("loginServer")]
        public string LoginServer { get; set; }
        [JsonPropertyName("adminUserEnabled")]
        public bool AdminUserEnabled { get; set; }
    }
}
