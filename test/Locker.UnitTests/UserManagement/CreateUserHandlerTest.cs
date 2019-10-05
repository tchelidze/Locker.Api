using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Auth.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using Locker.Domain.Features.UserManagement.CreateUser;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;


namespace Locker.UnitTests.UserManagement
{
    [TestFixture]
    public class CreateUserHandlerTest
    {
        private Mock<IUserRepository> _userRepository;
        private CreateUserHandler _sut;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _sut = new CreateUserHandler(_userRepository.Object);
        }

        [Test]
        public async Task WhenCommandIsHandlerThenUserShouldBeCreated()
        {
            var createUserCommand = new CreateUserCommand
            {
                Password = "123212",
                Permissions = new[]
                {
                    new UserPermission
                    {
                        Resource = "R1",
                        AccessRight = ResourceAccessRight.Create
                    }
                },
                UserName = "test"
            };

            var createdUser = new User { Id = "332" };

            _userRepository
                .Setup(it => it.TryAddUser(It.Is<User>(user => user.UserName == createUserCommand.UserName)))
                .ReturnsAsync(createdUser);

            var result = await _sut.Handle(createUserCommand, CancellationToken.None).ConfigureAwait(false);

            result.ResultType.ShouldBe(ExecutionResultType.Ok);
            result.Value.Id.ShouldBe(createdUser.Id);

            _userRepository.Verify(it => it.TryAddUser(It.Is<User>(user =>
                user.UserName == createUserCommand.UserName &&
                user.Permissions.Length == 1 &&
                user.Permissions[0].Resource == createUserCommand.Permissions[0].Resource &&
                user.Permissions[0].AccessRight == createUserCommand.Permissions[0].AccessRight &&
                !string.IsNullOrEmpty(user.PasswordHash))));
        }

        [Test]
        public async Task WhenUserWithSameUsernameAlreadyExistsItShouldReturnError()
        {
            var createUserCommand = new CreateUserCommand
            {
                Password = "123212",
                Permissions = new[]
                {
                    new UserPermission
                    {
                        Resource = "R1",
                        AccessRight = ResourceAccessRight.Create
                    }
                },
                UserName = "test"
            };


            _userRepository
                .Setup(it => it.TryAddUser(It.Is<User>(user => user.UserName == createUserCommand.UserName)))
                .ReturnsAsync((User)null);

            var result = await _sut.Handle(createUserCommand, CancellationToken.None).ConfigureAwait(false);

            result.ResultType.ShouldBe(ExecutionResultType.ValidationError);
        }
    }
}