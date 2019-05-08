using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Moq;
using QueueIT.Controllers.Users;
using QueueIT.Identity;
using QueueIT.Models;
using Shouldly;

namespace QueueITTests
{
    public class UsersControllerTests
    {
        private readonly Mock<UserManager<QueueItUser>> _mockUserManager = GetMockUserManager();
        private readonly Mock<QueueItDbContext> _mockQueueItDbContext = new Mock<QueueItDbContext>();
        private readonly Mock<QueueItUserDbContext> _mockQueueItUserDbContext = new Mock<QueueItUserDbContext>();
        private readonly Mock<IServiceProvider> _mockServiceProvider = new Mock<IServiceProvider>();
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
        
        private UsersController GetUserController()
        {
            return new UsersController(_mockQueueItDbContext.Object, _mockQueueItUserDbContext.Object,
                _mockUserManager.Object)
            {
                ControllerContext =
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        RequestServices = _mockServiceProvider.Object
                    }
                }
            };
        }

        [Fact]
        public async void TestProfile_WithSuccess()
        {
            var userController = GetUserController();
            var mockUser = GetMockUser(true);
            
        }
    }
}