using System.ComponentModel.DataAnnotations;

namespace SecurityServer.Service.DTO.Up
{
    public class ApplicationDeleteDtoUp
    {
        [Required]
        public int Id { get; set; }
    }
}
