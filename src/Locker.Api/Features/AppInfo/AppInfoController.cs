using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Locker.Api.Features.AppInfo
{
    public class AppInfoController : Controller
    {
        private readonly string _assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        [AllowAnonymous, HttpGet("api/app/info")]
        public object AppVersion() => new
        {
            version = _assemblyVersion
        };
    }
}