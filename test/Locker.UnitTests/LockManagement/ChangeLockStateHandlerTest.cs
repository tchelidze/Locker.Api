using Locker.Domain.Features.LockManagement.Entities;
using Locker.Domain.Features.LockManagement.Locking;
using Locker.Domain.Features.LockManagement.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;


namespace Locker.UnitTests.LockManagement
{
    [TestFixture]
    public class ChangeLockStateHandlerTest
    {
        private Mock<ILockRepository> _lockRepositoryMock;
        private ChangeLockStateHandler _sut;

        [SetUp]
        public void Setup()
        {
            _lockRepositoryMock = new Mock<ILockRepository>();
            _sut = new ChangeLockStateHandler(_lockRepositoryMock.Object);
        }

        [Test]
        public async Task WhenLockIdIsInvalidItShouldReturnError()
        {
            const string lockId = "123";

            _lockRepositoryMock
                .Setup(it => it.TryAddLocking(lockId, It.IsAny<LockingHistoryItem>()))
                .ReturnsAsync((Lock)null);

            var result = await _sut.Handle(new ChangeLockStateCommand
            {
                LockId = lockId,
                State = LockingState.Unlocked
            }, CancellationToken.None).ConfigureAwait(false);

            result.ResultType.ShouldBe(ExecutionResultType.NotFound);
        }

        [Test]
        public async Task WhenLockIdIsValidItShouldAddLocking()
        {
            const string lockId = "123";
            var updatedLock = new Lock
            {
                Id = "441",
                FriendlyName = "Test_Lock"
            };

            _lockRepositoryMock
                .Setup(it => it.TryAddLocking(lockId, It.IsAny<LockingHistoryItem>()))
                .ReturnsAsync(updatedLock);

            var changeLockStateCommand = new ChangeLockStateCommand
            {
                LockId = lockId,
                State = LockingState.Unlocked,
                UserId = "431"
            };

            var result = await _sut.Handle(changeLockStateCommand, CancellationToken.None).ConfigureAwait(false);

            _lockRepositoryMock.Verify(it => it.TryAddLocking(lockId,
                It.Is<LockingHistoryItem>(item => item.State == changeLockStateCommand.State
                                                  && item.UserId == changeLockStateCommand.UserId)));

            result.ResultType.ShouldBe(ExecutionResultType.Ok);
            result.Value.Id.ShouldBe(updatedLock.Id);
            result.Value.FriendlyName.ShouldBe(updatedLock.FriendlyName);
        }
    }
}