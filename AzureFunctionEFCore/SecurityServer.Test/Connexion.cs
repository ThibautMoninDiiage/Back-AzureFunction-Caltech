using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Service.Interfaces;

namespace SecurityServer.Test
{
    [TestClass]
    public class Connexion
    {

        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public Connexion(IUserService userService,IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [TestMethod]
        public async void TestGenerateCodeGrant()
        {
            UserDtoUp userDtoUp = new UserDtoUp() { Mail = "tristandevoille@gmail.com", Password = "Azerty@123" , Url = ""};

            string resultGrant = await _userService.Authenticate(userDtoUp);

            Assert.IsNotNull(resultGrant);
        }

        [TestMethod]
        public void TestGenerateToken()
        {
            string token = _jwtService.GenerateJwtToken(1,1);

            Assert.IsNotNull(token);
        }

        [TestMethod]
        public async void TestConnexionAZ()
        {
            UserDtoUp userDtoUp = new UserDtoUp() { Mail = "tristandevoille@gmail.com", Password = "Azerty@123", Url = "" };

            string resultGrant = await _userService.Authenticate(userDtoUp);

            UserDtoDown userDtoDown = await _userService.GetToken(resultGrant);

            Assert.IsNotNull(userDtoDown.Token);
        }
    }
}