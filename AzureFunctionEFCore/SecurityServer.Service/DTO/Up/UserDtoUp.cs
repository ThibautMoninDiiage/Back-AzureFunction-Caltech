using System.ComponentModel.DataAnnotations;

namespace SecurityServer.Service.DTO.Up
{
    public class UserDtoUp
    {
        [Required]
        public string? Mail { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? Url { get; set; }
    }
}
