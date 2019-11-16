using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Auth.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using Locker.Domain.Features.UserManagement.AddPermission;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Locker.UnitTests.UserManagement
{
    [TestFixture]
    public class AddUserPermissionHandlerTest
    {
        private Mock<IUserRepository> _userRepository;
        private AddUserPermissionHandler _sut;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _sut = new AddUserPermissionHandler(_userRepository.Object);
        }

        [Test]
        public async Task WhenUserIdIsInvalidItShouldReturnError()
        {
            Assert.Fail();
            var userId = "123";

            _userRepository
                .Setup(it => it.Get(userId))
                .ReturnsAsync((User)null);

            var result = await _sut.Handle(new AddUserPermissionCommand
            {
                UserId = userId,
                Permissions = new UserPermission[0]
            }, CancellationToken.None).ConfigureAwait(false);

            result.ResultType.ShouldBe(ExecutionResultType.NotFound);
        }

        [Test]
        public async Task WhenUserHasSamePermissionItShouldBeOverriden()
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Permissions = new[]
                {
                    new UserPermission
                    {
                        Resource = "R1",
                        AccessRight = ResourceAccessRight.Create
                    },
                    new UserPermission
                    {
                        Resource = "R2",
                        AccessRight = ResourceAccessRight.Update
                    }
                }
            };

            _userRepository.Setup(it => it.Get(user.Id)).ReturnsAsync(user);
            _userRepository.Setup(it => it.SetPermissions(user.Id, It.IsAny<UserPermission[]>())).ReturnsAsync(user);

            var result = await _sut.Handle(new AddUserPermissionCommand
            {
                UserId = user.Id,
                Permissions = new[]
                {
                    new UserPermission
                    {
                        Resource = "R1",
                        AccessRight = ResourceAccessRight.Delete
                    },
                    new UserPermission
                    {
                        Resource = "R3",
                        AccessRight = ResourceAccessRight.All | ResourceAccessRight.Update
                    },
                }
            }, CancellationToken.None).ConfigureAwait(false);

            result.ResultType.ShouldBe(ExecutionResultType.Ok);
            result.Value.Id.ShouldBe(user.Id);

            _userRepository.Verify(it => it.SetPermissions(user.Id, It.Is<UserPermission[]>(userPermissions =>
                    userPermissions.Length == 3 &&
                    userPermissions.Any(res => res.Resource == "R1" && res.AccessRight == ResourceAccessRight.Delete) &&
                    userPermissions.Any(res => res.Resource == "R2" && res.AccessRight == ResourceAccessRight.Update) &&
                    userPermissions.Any(res => res.Resource == "R3" && res.AccessRight == (ResourceAccessRight.All | ResourceAccessRight.Update))
                )),
                Times.Once);
        }
    }
}