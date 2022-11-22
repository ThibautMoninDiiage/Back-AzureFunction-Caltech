using System.ComponentModel.DataAnnotations.Schema;
using AzureFunction.Models.BaseModels;

namespace AzureFunction.Models
{
    [Table("Users")]
    public class User : BaseModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string? Mail { get; set; }
        public Role? Roles { get; set; }
        public int? IdRole { get; set; }
    }
}