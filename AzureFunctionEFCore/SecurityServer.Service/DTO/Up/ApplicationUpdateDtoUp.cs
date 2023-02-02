using SecurityServer.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace SecurityServer.Service.DTO.Up
{
    public class ApplicationUpdateDtoUp
    {
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Url { get; set; }

        public string? Description { get; set; }
    }
}
