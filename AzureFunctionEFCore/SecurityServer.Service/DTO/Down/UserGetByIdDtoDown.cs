namespace SecurityServer.Service.DTO.Down
{
    public class UserGetByIdDtoDown
    {
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Mail { get; set; }
        public string? Username { get; set; }
        public string? Avatar { get; set; }
        public List<ApplicationByUserDtoDown>? Applications { get; set; }
    }
}
