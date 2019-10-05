using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Locker.Domain.Features.Auth.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Locker.Api.Web.Filters
{
    public class CrudApiFilterAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// It can contain placeholders which will be bound from route data. Ex. Users@{userId}
        /// </summary>
        private readonly string _resourceName;
        private readonly ResourceAccessRight _requiredAccessRight;

        public CrudApiFilterAttribute(string resourceName, ResourceAccessRight requiredAccessRight)
        {
            _resourceName = resourceName;
            _requiredAccessRight = requiredAccessRight;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }

            var (rootResource, subResource) = GetResourceName(context);

            if (HasAccessToResource(user.Claims.FirstOrDefault(it => it.Type == rootResource)) ||
                HasAccessToResource(user.Claims.FirstOrDefault(it => it.Type == subResource)))
            {
                return Task.CompletedTask;
            }

            context.Result = new ForbidResult();

            return Task.CompletedTask;
        }

        private bool HasAccessToResource(Claim claim)
        {
            if (claim == null)
            {
                return false;
            }

            var accessRight = (ResourceAccessRight)Enum.Parse(typeof(ResourceAccessRight), claim.Value);

            return (accessRight & ResourceAccessRight.All) == ResourceAccessRight.All || (accessRight & _requiredAccessRight) == _requiredAccessRight;
        }

        /// <summary>
        /// When resource requirement has no placeholder, result contains no sub resource.
        ///    Example. Resource: "Users", Result: (rootResource: "Users", "subResource" : null)
        /// When resource requirement has placeholder, result contains sub resource bound to RouteData.
        ///    Example. Resource: "Users@{userId}", Result: (rootResource: "Users", "subResource" : User@12) where 12 is bound from route data
        /// </summary>
        private (string rootResource, string subResource) GetResourceName(AuthorizationFilterContext context)
        {
            if (!_resourceName.Contains("@"))
            {
                return (_resourceName, null);
            }

            var resourceName = new StringBuilder(_resourceName);

            foreach (Match match in Regex.Matches(_resourceName, @"\{(?<value>\w+)}"))
            {
                var value = match.Groups["value"].Value;
                var routeValue = context.RouteData.Values[value].ToString();
                resourceName.Replace(match.Value, routeValue);
            }

            return (_resourceName.Substring(0, _resourceName.IndexOf("@")), resourceName.ToString());
        }
    }
}