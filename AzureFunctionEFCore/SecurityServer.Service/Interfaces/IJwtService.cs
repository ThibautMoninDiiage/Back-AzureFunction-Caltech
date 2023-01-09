using SecurityServer.Models.Models;

namespace SecurityServer.Service.Interfaces
{
    public interface IJwtService
    {
        public string generateJwtToken(int idUser, int idRole);
        public byte[] GenerateSalt();
        public string HashPasswordWithSalt(string password, byte[] salt);
    }
}
