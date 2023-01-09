using System.ComponentModel.DataAnnotations;

namespace SecurityServer.Models.Models
{
    public class ApplicationUserRole
    {
        public User? User { get; set; }
        public int UserId { get; set; }
        public Role? Role { get; set; }
        public int RoleId { get; set; }
        public Application? Application { get; set; }
        public int ApplicationId { get; set; }
    }
}
