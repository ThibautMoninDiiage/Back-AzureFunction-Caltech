using Moq;
using SecurityServer.Contract.Repositories;
using SecurityServer.Contract.UnitOfWork;
using SecurityServer.DataAccess.Repositories;
using SecurityServer.Models;
using SecurityServer.Models.Models;
using SecurityServer.Service;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;

namespace SecurityServer.Test
{
    [TestClass]
    public class Connexion
    {

        private readonly UserService? _userService;
        private readonly JwtService? _jwtService;
        private readonly ApiSettings _apiSettings;

        public Connexion()
        {
            Mock<IUnitOfWork> _uow = new Mock<IUnitOfWork>();
            Mock<IApplicationRepository> _applicationRepository = new Mock<IApplicationRepository>();
            Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
            Mock<IApplicationUserRoleRepository> _applicationUserRepository = new Mock<IApplicationUserRoleRepository>();
            Mock<IGrantRepository> _grantRepository = new Mock<IGrantRepository>();
            Mock<IRoleRepository> _roleRepository = new Mock<IRoleRepository>();
            _apiSettings = new ApiSettings();
            _apiSettings.JwtIssuer = "mydomain";
            _apiSettings.JwtAudience = "localhost";

            _jwtService = new JwtService(_apiSettings);

            _uow.Setup(m => m.ApplicationRepository).Returns(_applicationRepository.Object);
            _uow.Setup(m => m.UserRepository.Get(x => x.Mail == "tristandevoille@gmail.com")).Returns(new User() {Id=1,Mail="tristandevoille@gmail.com",FirstName="Tristan",LastName="Devoille",Avatar="",Username="td",Password= "A8/voUjZ98zHaKO6Q4YEKsTWICUR0y0PRw9Z7QC31qs=", Salt= "e9oK6N1mqpdFkJSokr4Oxw==" });
            _uow.Setup(m => m.ApplicationUserRoleRepository).Returns(_applicationUserRepository.Object);
            _uow.Setup(m => m.GrantRepository).Returns(_grantRepository.Object);
            _uow.Setup(m => m.RoleRepository).Returns(_roleRepository.Object);

            //_uow.Setup(m => m.UserRepository.Add(new User() { Id = 1, FirstName = "Tristan", LastName = "Devoille", Avatar = "", Mail = "tristandevoille@gmail.com", Password = "A8/voUjZ98zHaKO6Q4YEKsTWICUR0y0PRw9Z7QC31qs=", Username = "td", Salt = "e9oK6N1mqpdFkJSokr4Oxw==" }));

            _userService = new UserService(_uow.Object, _jwtService);
        }

        [TestMethod]
        public void TestGenerateCodeGrant()
        {
            

            UserDtoUp userDtoUp = new UserDtoUp() { Mail = "tristandevoille@gmail.com", Password = "Azerty@123" , Url = ""};

            string resultGrant = _userService.Authenticate(userDtoUp).Result;

            Assert.IsNotNull(resultGrant);
        }

        [TestMethod]
        public void TestGenerateToken()
        {
            string token = _jwtService.GenerateJwtToken(1,1);

            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void TestConnexionAZ()
        {
            UserDtoUp userDtoUp = new UserDtoUp() { Mail = "tristandevoille@gmail.com", Password = "Azerty@123", Url = "" };

            string resultGrant = _userService.Authenticate(userDtoUp).Result;

            resultGrant = resultGrant.Split('=').ToString();

            UserDtoDown userDtoDown = _userService.GetToken(resultGrant).Result;

            Assert.IsNotNull(userDtoDown.Token);
        }
    }
}