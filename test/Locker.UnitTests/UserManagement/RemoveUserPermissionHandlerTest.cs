using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Auth.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using Locker.Domain.Features.UserManagement.RemovePermission;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Locker.UnitTests.UserManagement
{
    [TestFixture]
    public class RemoveUserPermissionHandlerTest
    {
        private Mock<IUserRepository> _userRepository;
        private RemoveUserPermissionHandler _sut;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _sut = new RemoveUserPermissionHandler(_userRepository.Object);
        }

        [Test]
        public async Task WhenUserIdIsInvalidItShouldReturnError()
        {
            var userId = "123";

            _userRepository
                .Setup(it => it.RemovePermissions(userId, It.IsAny<string[]>()))
                .ReturnsAsync((User)null);

            var result = await _sut.Handle(new RemoveUserPermissionCommand
            {
                UserId = userId,
                Resources = new string[0]
            }, CancellationToken.None).ConfigureAwait(false);

            result.ResultType.ShouldBe(ExecutionResultType.NotFound);
        }

        [Test]
        public async Task WhenUserHasMatchingPermissionItShouldBeDeleted()
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

            _userRepository.Setup(it => it.RemovePermissions(user.Id, It.IsAny<string[]>())).ReturnsAsync(user);

            var result = await _sut.Handle(new RemoveUserPermissionCommand
            {
                UserId = user.Id,
                Resources = new[] { "R1" }
            }, CancellationToken.None).ConfigureAwait(false);

            result.ResultType.ShouldBe(ExecutionResultType.Ok);
            result.Value.Id.ShouldBe(user.Id);

            _userRepository.Verify(it => it.RemovePermissions(user.Id, It.Is<string[]>(resources =>
                    resources.Length == 1 && resources[0] == "R1")),
                Times.Once);
        }
    }
}