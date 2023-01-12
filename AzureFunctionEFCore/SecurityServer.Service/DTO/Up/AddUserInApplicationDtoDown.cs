using SecurityServer.Service.DTO.Down;

namespace SecurityServer.Service.DTO.Up
{
    public class AddUserInApplicationDtoDown
    {
        public int UserId { get; set; }
        public int ApplicationId { get; set; }
        public RoleUserDtoDown? Role { get; set; }
    }
}
