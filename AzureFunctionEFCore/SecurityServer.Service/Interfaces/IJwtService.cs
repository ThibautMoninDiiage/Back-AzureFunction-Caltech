namespace SecurityServer.Service.Interfaces
{
    public interface IJwtService
    {
        public string GenerateJwtToken(int idUser, int idRole);
        public byte[] GenerateSalt();
        public string HashPasswordWithSalt(string password, byte[] salt);
        public Guid GenerateGrantCode();
    }
}
