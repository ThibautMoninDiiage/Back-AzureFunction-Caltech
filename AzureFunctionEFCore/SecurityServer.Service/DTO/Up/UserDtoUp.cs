using System.ComponentModel.DataAnnotations;

namespace SecurityServer.Service.DTO.Up
{
    public class UserDtoUp
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
