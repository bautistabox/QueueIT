using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using QueueIT.Controllers;
using QueueIT.Identity;
using QueueIT.InputModels;
using QueueIT.Models;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace QueueITTests
{
    public class HomeControllerTests
    {
        private readonly Mock<UserManager<QueueItUser>> _mockUserManager =
            GetMockUserManager();

        private readonly Mock<IUserClaimsPrincipalFactory<QueueItUser>> _mockUserClaimsPrincipalFactory =
            new Mock<IUserClaimsPrincipalFactory<QueueItUser>>();

        private readonly Mock<IAuthenticationService> _mockAuthenticationService = new Mock<IAuthenticationService>();

        private readonly Mock<IServiceProvider> _mockServiceProvider = new Mock<IServiceProvider>();
            
        private readonly Mock<IUrlHelperFactory> _mockUrlHelperFactory = new Mock<IUrlHelperFactory>();

        private readonly Mock<ITempDataDictionaryFactory> _mockTempDataDictionaryFactory =
            new Mock<ITempDataDictionaryFactory>();

        private readonly Mock<QueueItDbContext> _mockQueueItDbContext = new Mock<QueueItDbContext>();
        
        private static Mock<UserManager<QueueItUser>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<QueueItUser>>();
            return new Mock<UserManager<QueueItUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        private static QueueItUser GetMockUser(bool EmailVerification)
        {
            return new QueueItUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "mockUsername",
                PasswordHash = "mockPasswordHash",
                EmailConfirmed = EmailVerification
            };
        }

        private static LoginInputModel GetMockLoginInputModel(string mockUsername, string mockPassword)
        {
            return new LoginInputModel
            {
                LoginUserName = mockUsername,
                LoginPassword = mockPassword
            };
        }

        private HomeController GetHomeController()
        {
            return new HomeController(_mockUserManager.Object, _mockUserClaimsPrincipalFactory.Object, _mockQueueItDbContext.Object)
            {
                ControllerContext = {HttpContext = new DefaultHttpContext()
                {
                    RequestServices = _mockServiceProvider.Object
                }}
            };
        }

        [Fact]
        public async void TestLogin_WithSuccess()
        {
            var mockLoginInputModel = GetMockLoginInputModel("mockUsername", "mockPassword");
            var homeController = GetHomeController();
            var mockUser = GetMockUser(true);
            var mockPrincipal = new ClaimsPrincipal();
            
            _mockUserManager
                .Setup(m => m.FindByNameAsync(mockLoginInputModel.LoginUserName))
                .ReturnsAsync(mockUser);

            _mockUserManager
                .Setup(m => m.CheckPasswordAsync(mockUser, mockLoginInputModel.LoginPassword))
                .ReturnsAsync(true);

            _mockUserManager
                .Setup(m => m.IsEmailConfirmedAsync(mockUser))
                .ReturnsAsync(true);

            _mockUserClaimsPrincipalFactory
                .Setup(m => m.CreateAsync(mockUser))
                .ReturnsAsync(mockPrincipal);

            _mockAuthenticationService
                .Setup(m => m.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            _mockServiceProvider
                .Setup(m => m.GetService(typeof(IAuthenticationService)))
                .Returns(_mockAuthenticationService.Object);
            
            _mockServiceProvider
                .Setup(m => m.GetService(typeof(IUrlHelperFactory)))
                .Returns(_mockUrlHelperFactory.Object);

            
            var actualResult = await homeController.Login(mockLoginInputModel);
            
            // VERIFY
            _mockUserManager.Verify(m => m.FindByNameAsync(mockLoginInputModel.LoginUserName));
            _mockUserManager.Verify(m => m.CheckPasswordAsync(mockUser, mockLoginInputModel.LoginPassword));
            _mockUserManager.Verify(m => m.IsEmailConfirmedAsync(mockUser));
            _mockUserClaimsPrincipalFactory.Verify(m => m.CreateAsync(mockUser));

            actualResult.ShouldNotBeNull();
            actualResult.ShouldBeOfType<RedirectToActionResult>();
        }

        [Fact]
        public async void TestLogin_WithBadPassword()
        {           
            var mockLoginInputModel = GetMockLoginInputModel("mockUsername", "mockPassword");
            var mockUser = GetMockUser(true);
            var homeController = GetHomeController();
            
            _mockUserManager
                .Setup(m => m.FindByNameAsync(mockLoginInputModel.LoginUserName))
                .ReturnsAsync(mockUser);

            _mockUserManager
                .Setup(m => m.CheckPasswordAsync(mockUser, mockLoginInputModel.LoginPassword))
                .ReturnsAsync(false);
            
            _mockServiceProvider
                .Setup(m => m.GetService(typeof(ITempDataDictionaryFactory)))
                .Returns(_mockTempDataDictionaryFactory.Object);
            
            
            await homeController.Login(mockLoginInputModel);
            var actualResult = await homeController.Login(mockLoginInputModel);

            _mockUserManager.Verify(m => m.FindByNameAsync(mockLoginInputModel.LoginUserName));
            _mockUserManager.Verify(m => m.CheckPasswordAsync(mockUser, mockLoginInputModel.LoginPassword));
            
            actualResult.ShouldNotBeNull();
            homeController.ModelState.Count.ShouldNotBe(0);
            actualResult.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async void TestLogin_WithBadUsername()
        {
            var homeController = GetHomeController();
            var mockLoginInputModel = GetMockLoginInputModel("mockUsername", "mockPassword");
            
            _mockUserManager
                .Setup(m => m.FindByNameAsync(mockLoginInputModel.LoginUserName))
                .ReturnsAsync((QueueItUser) null);
            
            _mockServiceProvider
                .Setup(m => m.GetService(typeof(ITempDataDictionaryFactory)))
                .Returns(_mockTempDataDictionaryFactory.Object);

            var actualResult = await homeController.Login(mockLoginInputModel);
            
            _mockUserManager.Verify(m => m.FindByNameAsync(mockLoginInputModel.LoginUserName));

            actualResult.ShouldNotBeNull();
            actualResult.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async void TestLogin_WithUnconfirmedEmail()
        {
            var homeController = GetHomeController();
            var mockUser = GetMockUser(false);
            var mockLoginInputModel = GetMockLoginInputModel("mockUsername", "mockPassword");
            
            _mockUserManager
                .Setup(m => m.FindByNameAsync(mockLoginInputModel.LoginUserName))
                .ReturnsAsync(mockUser);
            
            _mockUserManager
                .Setup(m => m.CheckPasswordAsync(mockUser, mockLoginInputModel.LoginPassword))
                .ReturnsAsync(true);

            _mockUserManager
                .Setup(m => m.IsEmailConfirmedAsync(mockUser))
                .ReturnsAsync(false);
            
            _mockServiceProvider
                .Setup(m => m.GetService(typeof(ITempDataDictionaryFactory)))
                .Returns(_mockTempDataDictionaryFactory.Object);

            var actualResult = await homeController.Login(mockLoginInputModel);
            
            _mockUserManager.Verify(m => m.FindByNameAsync(mockLoginInputModel.LoginUserName));         
            _mockUserManager.Verify(m => m.CheckPasswordAsync(mockUser, mockLoginInputModel.LoginPassword)); 
            _mockUserManager.Verify(m => m.IsEmailConfirmedAsync(mockUser));

            actualResult.ShouldNotBeNull();
            actualResult.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async void TestLogin_WithModelState_Invalid_Username()
        {
            var mockLoginInputModel = GetMockLoginInputModel("", "mockPassword");
            var homeController = GetHomeController();
            
            _mockServiceProvider
                .Setup(m => m.GetService(typeof(ITempDataDictionaryFactory)))
                .Returns(_mockTempDataDictionaryFactory.Object);
            
            var actualResult = await homeController.Login(mockLoginInputModel);
            
            actualResult.ShouldNotBeNull();
            actualResult.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async void TestLogin_WithModelState_Invalid_Password()
        {
            var mockLoginInputModel = GetMockLoginInputModel("mockUsername", "");
            var homeController = GetHomeController();
            
            _mockServiceProvider
                .Setup(m => m.GetService(typeof(ITempDataDictionaryFactory)))
                .Returns(_mockTempDataDictionaryFactory.Object);
            
            var actualResult = await homeController.Login(mockLoginInputModel);
            
            actualResult.ShouldNotBeNull();
            actualResult.ShouldBeOfType<ViewResult>();
        }
        
    }
}