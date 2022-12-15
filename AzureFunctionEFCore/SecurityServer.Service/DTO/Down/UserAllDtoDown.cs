using SecurityServer.Models.Models;

namespace SecurityServer.Service.DTO.Down
{
    public class UserAllDtoDown
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Avatar { get; set; }
        public string? Mail { get; set; }
        public RoleUserDtoDown Role { get; set; }

    }
}
