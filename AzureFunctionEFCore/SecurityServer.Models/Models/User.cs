using SecurityServer.Models.Models.BaseModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityServer.Models.Models
{
    [Table("Users")]
    public class User : BaseModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string? Mail { get; set; }
        public Role? Role { get; set; }
        public int? IdRole { get; set; }
    }
}
