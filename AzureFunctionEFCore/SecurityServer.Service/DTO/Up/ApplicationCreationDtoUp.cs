using SecurityServer.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace SecurityServer.Service.DTO.Up
{
    public class ApplicationCreationDtoUp
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Url { get; set; }

        [Required]
        public string? Description { get; set; }
    }
}
