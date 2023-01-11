using SecurityServer.Models.Models;

namespace SecurityServer.Models.Models
{
    public class Grant
    {
        public User? User { get; set; }
        public int UserId { get; set; }
        public Application? Application { get; set; }
        public int ApplicationId { get; set; }
        public Guid Code { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
