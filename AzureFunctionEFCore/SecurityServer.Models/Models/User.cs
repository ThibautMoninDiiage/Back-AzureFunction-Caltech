using SecurityServer.Models.Models.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityServer.Models.Models
{
    [Table("Users")]
    public class User : BaseModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string? Mail { get; set; }
        [Required]
        public string? Salt { get; set; }
        public Role? Role { get; set; }
        [Required]
        public int? IdRole { get; set; }
    }
}
