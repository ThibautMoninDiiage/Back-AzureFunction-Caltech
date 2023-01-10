using SecurityServer.Models.Models.BaseModels;
using System.Text.Json.Serialization;

namespace SecurityServer.Models.Models
{
    public class Claim : BaseModel
    {
        public string? Name { get; set; }
        [JsonIgnore]
        public List<Application> Applications { get; set; }
        [JsonIgnore]
        public List<User> Users { get; set; }
    }
}
