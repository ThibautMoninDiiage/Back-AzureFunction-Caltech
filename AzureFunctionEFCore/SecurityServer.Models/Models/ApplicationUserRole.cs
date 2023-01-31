using System.ComponentModel.DataAnnotations;

namespace SecurityServer.Models.Models
{
    public class ApplicationUserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int ApplicationId { get; set; }
    }
}
