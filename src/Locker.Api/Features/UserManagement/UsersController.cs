using Locker.Api.Features.Shared;
using Locker.Api.Features.UserManagement.Models;
using Locker.Api.Web.Filters;
using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Auth.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using Locker.Domain.Features.UserManagement.AddPermission;
using Locker.Domain.Features.UserManagement.CreateUser;
using Locker.Domain.Features.UserManagement.RemovePermission;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Threading.Tasks;

namespace Locker.Api.Features.UserManagement
{
    [ValidateModelStateFilter]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;

        public UsersController(IMediator mediator, IUserRepository userRepository)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        [HttpPost("api/users"), CrudApiFilter("Users", ResourceAccessRight.Create)]
        public async Task<IActionResult> Create([BindRequired, FromBody] CreateUserApiInput input)
        {
            var result = await _mediator.Send(new CreateUserCommand
            {
                UserName = input.UserName,
                Password = input.Password,
                Permissions = input.Permissions?.GroupBy(it => it.Resource).Select(it => new UserPermission
                {
                    Resource = it.Key,
                    AccessRight = it.Aggregate(it.First().AccessRight, (acc, cur) => acc | cur.AccessRight)
                }).ToArray()
            }).ConfigureAwait(false);

            return new ExecutionResult<UserReadModel>
            {
                Value = result.Value == null ? null : new UserReadModel(result.Value),
                ResultType = result.ResultType,
                Message = result.Message
            }.ToActionResult();
        }

        [HttpGet("api/users/{userId:objectId}"), CrudApiFilter("Users@{userId}", ResourceAccessRight.Read)]
        public async Task<IActionResult> Read([BindRequired, FromRoute] string userId)
        {
            var user = await _userRepository.Get(userId).ConfigureAwait(false);

            if (user == null)
            {
                return new ExecutionResult<UserReadModel>
                {
                    ResultType = ExecutionResultType.NotFound,
                    Message = "User not found"
                }.ToActionResult();
            }

            return new ExecutionResult<UserReadModel>
            {
                ResultType = ExecutionResultType.Ok,
                Value = new UserReadModel(user)
            }.ToActionResult();
        }

        [HttpPost("api/users/{userId:objectId}/permissions"), CrudApiFilter("Users@{userId}", ResourceAccessRight.Update)]
        public async Task<IActionResult> AddPermission([BindRequired, FromRoute] string userId, [BindRequired, FromBody]  AddUserPermissionApiInput input)
        {
            var result = await _mediator.Send(new AddUserPermissionCommand
            {
                UserId = userId,
                Permissions = input.Permissions.Select(it => new UserPermission
                {
                    Resource = it.Resource,
                    AccessRight = it.AccessRight
                }).ToArray()
            }).ConfigureAwait(false);

            return new ExecutionResult<UserReadModel>
            {
                Message = result.Message,
                ResultType = result.ResultType,
                Value = result.Value == null ? null : new UserReadModel(result.Value)
            }.ToActionResult();
        }

        [HttpDelete("api/users/{userId:objectId}/permissions"), CrudApiFilter("Users@{userId}", ResourceAccessRight.Update)]
        public async Task<IActionResult> RemovePermission([BindRequired, FromRoute] string userId, [BindRequired, FromBody] RemoveUserPermissionApiInput input)
        {
            var result = await _mediator.Send(new RemoveUserPermissionCommand
            {
                UserId = userId,
                Resources = input.Resources
            }).ConfigureAwait(false);

            return new ExecutionResult<UserReadModel>
            {
                Message = result.Message,
                ResultType = result.ResultType,
                Value = result.Value == null ? null : new UserReadModel(result.Value)
            }.ToActionResult();
        }
    }
}