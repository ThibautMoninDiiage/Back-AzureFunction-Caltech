using SecurityServer.Models.Models;

namespace SecurityServer.Service.DTO.Down
{
    public class UserDtoDown
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Mail { get; set; }
        public string Token { get; set; }


        public UserDtoDown(User user, string token)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Mail = user.Mail;
            Token = token;
        }
    }
}
