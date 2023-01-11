using SecurityServer.Models.Models.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SecurityServer.Models.Models
{
    public class Application : BaseModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Url { get; set; }
        [Required]
        public string? Description { get; set; }
        public string? RedirectUri { get; set; }
        [JsonIgnore]
        public List<Claim>? Claims { get; set; }
        public List<ApplicationUserRole>? ApplicationUserRoles { get; set; }
    }
}
