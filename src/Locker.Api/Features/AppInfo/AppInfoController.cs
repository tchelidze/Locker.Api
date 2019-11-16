using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locker.Api.Features.AppInfo
{
    public class AppInfoController : Controller
    {
        private readonly string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        [AllowAnonymous, HttpGet("api/app/info")]
        public object AppVersion() => new
        {
            version = AssemblyVersion
        };
    };
}