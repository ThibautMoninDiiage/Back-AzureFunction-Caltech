using System.ComponentModel.DataAnnotations;

namespace SecurityServer.Service.DTO.Down
{
    public class ApplicationDtoDown
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Url { get; set; }
        [Required]
        public string? Description { get; set; }
        public string? SecretCode { get; set; }
        public List<UserByApplicationDtoDown>? Users { get; set; }
    }
}
