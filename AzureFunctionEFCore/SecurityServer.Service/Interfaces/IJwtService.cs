using SecurityServer.Models.Models;

namespace SecurityServer.Service.Interfaces
{
    public interface IJwtService
    {
        public string generateJwtToken(User user);
        public byte[] GenerateSalt();
        public string HashPasswordWithSalt(string password, byte[] salt);
    }
}
