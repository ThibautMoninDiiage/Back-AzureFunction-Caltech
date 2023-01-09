using SecurityServer.Models.Models.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SecurityServer.Models.Models
{
    [Table("Users")]
    public class User : BaseModel
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        [JsonIgnore]
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string? Mail { get; set; }
        [Required]
        [JsonIgnore]
        public string? Salt { get; set; }
        public List<ApplicationUserRole>? ApplicationUserRoles { get; set; }
    }
}
