using SecurityServer.Models.Models.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityServer.Models.Models
{
    [Table("Roles")]
    public class Role : BaseModel
    {
        [Required]
        public string? Name { get; set; }
        public List<User>? Users { get; set; }
    }
}
