using Locker.Api.Web.Filters;
using Locker.Domain.Features.Auth.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;


namespace Locker.UnitTests.Web.Aut
{
    [TestFixture]
    public class CrudApiFilterTest
    {
        [Test]
        public async Task WhenUserIsNotAuthenticatedThenResultShouldntBeSet()
        {
            var sut = new CrudApiFilterAttribute("R1", ResourceAccessRight.Create);
            var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new GenericIdentity("", "BB"))
            }, new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());

            await sut.OnAuthorizationAsync(context).ConfigureAwait(false);
            context.Result.ShouldBeNull();
        }

        [Test]
        public async Task WhenUserHasAccessOnRootResourceThenFilterShouldPass()
        {
            var sut = new CrudApiFilterAttribute("Locks@{lockId}", ResourceAccessRight.Update);
            var userIdentity = new GenericIdentity("User_1", "Normal");
            userIdentity.AddClaim(new Claim("Locks", ((int)(ResourceAccessRight.Update | ResourceAccessRight.Read)).ToString()));

            var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(userIdentity)
            }, new RouteData(RouteValueDictionary.FromArray(new[]
            {
                new KeyValuePair<string, object>("lockId", "123AdS"),
                new KeyValuePair<string, object>("userId", "14")
            })), new ActionDescriptor()), new List<IFilterMetadata>());

            await sut.OnAuthorizationAsync(context).ConfigureAwait(false);
            context.Result.ShouldBeNull();
        }


        [Test]
        public async Task WhenUserHasAccessToSubResourceThenFilterShouldPass()
        {
            var sut = new CrudApiFilterAttribute("Locks@{lockId}", ResourceAccessRight.Update);
            var userIdentity = new GenericIdentity("User_1", "Normal");
            userIdentity.AddClaim(new Claim("Locks@123AdS", ((int)(ResourceAccessRight.Update | ResourceAccessRight.Read)).ToString()));

            var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(userIdentity)
            }, new RouteData(RouteValueDictionary.FromArray(new[]
            {
                new KeyValuePair<string, object>("lockId", "123AdS"),
                new KeyValuePair<string, object>("userId", "14")
            })), new ActionDescriptor()), new List<IFilterMetadata>());

            await sut.OnAuthorizationAsync(context).ConfigureAwait(false);
            context.Result.ShouldBeNull();
        }

        [Test]
        public async Task WhenUserHasAccessToDifferentSubResourceThenFilterShouldPass()
        {
            var sut = new CrudApiFilterAttribute("Locks@{lockId}", ResourceAccessRight.Update);
            var userIdentity = new GenericIdentity("User_1", "Normal");
            userIdentity.AddClaim(new Claim("Locks@2", ((int)(ResourceAccessRight.Update | ResourceAccessRight.Read)).ToString()));

            var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(userIdentity)
            }, new RouteData(RouteValueDictionary.FromArray(new[]
            {
                new KeyValuePair<string, object>("lockId", "123AdS"),
                new KeyValuePair<string, object>("userId", "14")
            })), new ActionDescriptor()), new List<IFilterMetadata>());

            await sut.OnAuthorizationAsync(context).ConfigureAwait(false);
            context.Result.ShouldNotBeNull();
            context.Result.ShouldBeOfType<ForbidResult>();
        }

        [Test]
        public async Task WhenUserHasAccessToDifferentRootResourceThenFilterShouldPass()
        {
            var sut = new CrudApiFilterAttribute("Locks@{lockId}", ResourceAccessRight.Update);
            var userIdentity = new GenericIdentity("User_1", "Normal");
            userIdentity.AddClaim(new Claim("Users", ((int)ResourceAccessRight.All).ToString()));

            var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(userIdentity)
            }, new RouteData(RouteValueDictionary.FromArray(new[]
            {
                new KeyValuePair<string, object>("lockId", "123AdS"),
                new KeyValuePair<string, object>("userId", "14")
            })), new ActionDescriptor()), new List<IFilterMetadata>());

            await sut.OnAuthorizationAsync(context).ConfigureAwait(false);
            context.Result.ShouldNotBeNull();
            context.Result.ShouldBeOfType<ForbidResult>();
        }
    }
}