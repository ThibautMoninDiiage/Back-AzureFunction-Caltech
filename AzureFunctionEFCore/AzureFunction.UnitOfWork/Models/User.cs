using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AzureFunction.UnitOfWork.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string? Mail { get; set; }
        public Role? Roles { get; set; }
        public int? IdRole { get; set; }
    }
}