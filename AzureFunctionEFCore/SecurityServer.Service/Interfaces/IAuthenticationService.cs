namespace SecurityServer.Service.Interfaces
{
    public interface IAuthenticationService
    {
        bool VerifyToken(string token);
    }
}
