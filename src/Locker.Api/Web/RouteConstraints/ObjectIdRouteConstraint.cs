using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Locker.Api.Web.RouteConstraints
{
    public class ObjectIdRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values,
            RouteDirection routeDirection) =>
            values.TryGetValue(routeKey, out var value) &&
            value is string str &&
            Regex.IsMatch(str, @"^[a-f\d]{24}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
