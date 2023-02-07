using Moq;
using SecurityServer.Contract.Repositories;
using SecurityServer.Contract.UnitOfWork;
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

        //public Connexion()
        //{
        //    Mock<IUnitOfWork> _uow = new Mock<IUnitOfWork>();
        //    Mock<IApplicationRepository> _applicationRepository = new Mock<IApplicationRepository>();
        //    Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
        //    Mock<IApplicationUserRoleRepository> _applicationUserRepository = new Mock<IApplicationUserRoleRepository>();
        //    Mock<IGrantRepository> _grantRepository = new Mock<IGrantRepository>();
        //    Mock<IRoleRepository> _roleRepository = new Mock<IRoleRepository>();
        //    _apiSettings = new ApiSettings();
        //    _apiSettings.JwtIssuer = "mydomain";
        //    _apiSettings.JwtAudience = "localhost";

        //    _jwtService = new JwtService(_apiSettings);

        //    UserDtoUp userDtoUp = new UserDtoUp() { Mail = "f.f@f.com", Password = "ff", Url = "" };
        //    User useressai = new User() { Id = 1, Mail = "f.f@f.com", FirstName = "Tristan", LastName = "Devoille", Avatar = "", Username = "td", Password = "s3vQv+KRhP0toyQKrB5Ayi6uG+HYYFvELgTYI62vFks=", Salt = "9/ylJQacGQRWac2kfOdflw==" };
        //    Guid grantCode = _jwtService.GenerateGrantCode();

        //    _uow.Setup(m => m.ApplicationRepository.GetAsync(x => x.Id == 1, It.IsAny<CancellationToken>())).ReturnsAsync(new Application() { Id = 1, Name = "Serveur de sécurité", Url = "https://localhost:4200/home", Description = "coucoucoucou" });
        //    _uow.Setup(m => m.UserRepository.GetAsync(x => x.Mail == userDtoUp.Mail, It.IsAny<CancellationToken>())).ReturnsAsync(useressai);
        //    _uow.Setup(m => m.ApplicationUserRoleRepository.GetAsync(x => x.ApplicationId == 1 && x.UserId == useressai.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new ApplicationUserRole() { ApplicationId = 1, UserId = 1, RoleId = 1 });
        //    _uow.Setup(m => m.GrantRepository.GetAsync(x => x.UserId == useressai.Id && x.ApplicationId == 1, It.IsAny<CancellationToken>())).ReturnsAsync(new Grant() { ApplicationId = 1, UserId = 1, CreatedAt = DateTime.Now, Code = grantCode.ToString() });
        //    _uow.Setup(m => m.RoleRepository).Returns(_roleRepository.Object);

        //    _userService = new UserService(_uow.Object, _jwtService);
        //}

        [TestMethod]
        public void TestGenerateCodeGrant()
        {
            Mock<IUnitOfWork> _uow = new Mock<IUnitOfWork>();
            Mock<IApplicationRepository> _applicationRepository = new Mock<IApplicationRepository>();
            Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
            Mock<IApplicationUserRoleRepository> _applicationUserRepository = new Mock<IApplicationUserRoleRepository>();
            Mock<IGrantRepository> _grantRepository = new Mock<IGrantRepository>();
            Mock<IRoleRepository> _roleRepository = new Mock<IRoleRepository>();
            ApiSettings _apiSettings = new ApiSettings();
            _apiSettings.JwtIssuer = "mydomain";
            _apiSettings.JwtAudience = "localhost";

            CertificatSettings _certificatSettings = new CertificatSettings();
            _certificatSettings.CertificateName = "certificatetoken";
            _certificatSettings.ClientId = "4e9414cf-0bfd-4144-880c-ccff9e466553";
            _certificatSettings.TenantId = "14bc5219-40ca-4d62-a8e4-7c97c1236349";
            _certificatSettings.Secret = "woJ8Q~UaQLITEXeUaiyKoy1mOGTplvEj8K5WObS2";
            _certificatSettings.VaultUrl = "https://preprodkeyvaultgdeuxb.vault.azure.net/";


            JwtService _jwtService = new JwtService(_apiSettings,_certificatSettings);

            string secret = Guid.NewGuid().ToString();

            UserDtoUp userDtoUp = new UserDtoUp() { Mail = "f.f@f.com", Password = "ff", SecretCode = secret };
            User useressai = new User() { Id = 1, Mail = "f.f@f.com", FirstName = "Tristan", LastName = "Devoille", Avatar = "", Username = "td", Password = "s3vQv+KRhP0toyQKrB5Ayi6uG+HYYFvELgTYI62vFks=", Salt = "9/ylJQacGQRWac2kfOdflw==" };
            Application application = new Application() { Id = 1, Name = "Serveur de sécurité", Url = "https://localhost:4200/home", Description = "coucoucoucou" };
            Guid grantCode = _jwtService.GenerateGrantCode();
            Grant grant = new Grant() { ApplicationId = 1, UserId = 1, CreatedAt = DateTime.Now, Code = grantCode.ToString() };

            _uow.Setup(m => m.ApplicationRepository.GetAsync(x => x.SecretCode == userDtoUp.SecretCode, It.IsAny<CancellationToken>())).ReturnsAsync(application);
            _uow.Setup(m => m.UserRepository.GetAsync(x => x.Mail == userDtoUp.Mail, It.IsAny<CancellationToken>())).ReturnsAsync(useressai);
            _uow.Setup(m => m.ApplicationUserRoleRepository.GetAsync(x => x.ApplicationId == application.Id && x.UserId == useressai.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new ApplicationUserRole() { ApplicationId = 1, UserId = 1, RoleId = 1 });
            _uow.Setup(m => m.GrantRepository.GetAsync(x => x.UserId == useressai.Id && x.ApplicationId == application.Id, It.IsAny<CancellationToken>())).ReturnsAsync(grant);

            UserService _userService = new UserService(_uow.Object, _jwtService);

            GrantDtoDown resultGrant = _userService.Authenticate(userDtoUp).Result;

            Assert.IsNotNull(resultGrant.CodeGrant);
        }

        [TestMethod]
        public void TestGenerateToken()
        {
            ApiSettings _apiSettings = new ApiSettings();
            _apiSettings.JwtIssuer = "mydomain";
            _apiSettings.JwtAudience = "localhost";

            CertificatSettings _certificatSettings = new CertificatSettings();
            _certificatSettings.CertificateName = "certificatetoken";
            _certificatSettings.ClientId = "4e9414cf-0bfd-4144-880c-ccff9e466553";
            _certificatSettings.TenantId = "14bc5219-40ca-4d62-a8e4-7c97c1236349";
            _certificatSettings.Secret = "woJ8Q~UaQLITEXeUaiyKoy1mOGTplvEj8K5WObS2";
            _certificatSettings.VaultUrl = "https://preprodkeyvaultgdeuxb.vault.azure.net/";


            JwtService _jwtService = new JwtService(_apiSettings, _certificatSettings);

            string token = _jwtService.GenerateJwtToken(1,1);

            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void TestConnexionAZ()
        {
            Mock<IUnitOfWork> _uow = new Mock<IUnitOfWork>();
            Mock<IApplicationRepository> _applicationRepository = new Mock<IApplicationRepository>();
            Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
            Mock<IApplicationUserRoleRepository> _applicationUserRepository = new Mock<IApplicationUserRoleRepository>();
            Mock<IGrantRepository> _grantRepository = new Mock<IGrantRepository>();
            Mock<IRoleRepository> _roleRepository = new Mock<IRoleRepository>();
            ApiSettings _apiSettings = new ApiSettings();
            _apiSettings.JwtIssuer = "mydomain";
            _apiSettings.JwtAudience = "localhost";

            CertificatSettings _certificatSettings = new CertificatSettings();
            _certificatSettings.CertificateName = "certificatetoken";
            _certificatSettings.ClientId = "4e9414cf-0bfd-4144-880c-ccff9e466553";
            _certificatSettings.TenantId = "14bc5219-40ca-4d62-a8e4-7c97c1236349";
            _certificatSettings.Secret = "woJ8Q~UaQLITEXeUaiyKoy1mOGTplvEj8K5WObS2";
            _certificatSettings.VaultUrl = "https://preprodkeyvaultgdeuxb.vault.azure.net/";


            JwtService _jwtService = new JwtService(_apiSettings, _certificatSettings);

            string secret = Guid.NewGuid().ToString();

            UserDtoUp userDtoUp = new UserDtoUp() { Mail = "f.f@f.com", Password = "ff", SecretCode = secret };
            User useressai = new User() { Id = 1, Mail = "f.f@f.com", FirstName = "Tristan", LastName = "Devoille", Avatar = "", Username = "td", Password = "s3vQv+KRhP0toyQKrB5Ayi6uG+HYYFvELgTYI62vFks=", Salt = "9/ylJQacGQRWac2kfOdflw==" };
            Application application = new Application() { Id = 1, Name = "Serveur de sécurité", Url = "https://localhost:4200/home", Description = "coucoucoucou" };
            ApplicationUserRole applicationUserRole = new ApplicationUserRole() { ApplicationId = 1, UserId = 1, RoleId = 1 };
            Guid grantCode = _jwtService.GenerateGrantCode();
            string grantCodeString = grantCode.ToString();
            Grant grant = new Grant() { ApplicationId = 1, UserId = 1, CreatedAt = DateTime.Now, Code = grantCode.ToString() };

            _uow.Setup(m => m.ApplicationRepository.GetAsync(x => x.SecretCode == userDtoUp.SecretCode, It.IsAny<CancellationToken>())).ReturnsAsync(application);
            _uow.Setup(m => m.UserRepository.GetAsync(x => x.Mail == userDtoUp.Mail, It.IsAny<CancellationToken>())).ReturnsAsync(useressai);
            _uow.Setup(m => m.UserRepository.GetAsync(x => x.Id == applicationUserRole.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(useressai);
            _uow.Setup(m => m.ApplicationUserRoleRepository.GetAsync(x => x.ApplicationId == application.Id && x.UserId == useressai.Id, It.IsAny<CancellationToken>())).ReturnsAsync(applicationUserRole);
            _uow.Setup(m => m.ApplicationUserRoleRepository.GetAsync(x => x.ApplicationId == grant.ApplicationId && x.UserId == grant.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(applicationUserRole);
            _uow.Setup(m => m.GrantRepository.GetAsync(x => x.UserId == useressai.Id && x.ApplicationId == application.Id, It.IsAny<CancellationToken>())).ReturnsAsync(grant);
            _uow.Setup(m => m.GrantRepository.GetAsync(x => x.Code == grantCodeString, It.IsAny<CancellationToken>())).ReturnsAsync(grant);
            _uow.Setup(m => m.RoleRepository).Returns(_roleRepository.Object);

            UserService _userService = new UserService(_uow.Object, _jwtService);

            GrantDtoDown resultGrant = _userService.Authenticate(userDtoUp).Result;

            UserDtoDown userDtoDown = _userService.GetToken(resultGrant.CodeGrant).Result;

            Assert.IsNotNull(userDtoDown.Token);
        }

        [TestMethod]
        public void TestNul()
        {
            int test = 2;

            Assert.AreEqual(4, test + test);
        }
    }
}