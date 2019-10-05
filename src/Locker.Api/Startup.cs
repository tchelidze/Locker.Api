using FluentValidation.AspNetCore;
using Locker.Api.Configuration;
using Locker.Api.Features.Auth;
using Locker.Api.Features.Auth.Models;
using Locker.Api.Web.RouteConstraints;
using Locker.DataAccess.Features.Auth;
using Locker.DataAccess.Features.LockManagement;
using Locker.DataAccess.Features.Shared;
using Locker.Domain.Features.Auth.Repositories;
using Locker.Domain.Features.LockManagement.Repositories;
using Locker.Domain.Features.UserManagement.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.Conventions;
using System.Text;
using System.Text.Json;

namespace Locker.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var authOptions = Configuration.GetSection(nameof(AutSettings)).Get<AutSettings>();

            services.AddSingleton(authOptions);
            services.AddSingleton(Configuration.GetSection(nameof(MongoRepositorySettings)).Get<MongoRepositorySettings>());

            ConventionRegistry.Register("CamelCaseNameConvention", new ConventionPack
            {
                new CamelCaseElementNameConvention()
            }, type => true);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "JwtBearer";
                    options.DefaultChallengeScheme = "JwtBearer";
                })
                .AddJwtBearer("JwtBearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SigningKey)),

                        ValidateIssuer = true,
                        ValidIssuer = authOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = authOptions.Audience,

                        ValidateLifetime = true,
                    };
                });

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILockRepository, LockRepository>();

            services.AddScoped<DatabaseInitializer>();

            services.AddMediatR(typeof(CreateUserCommand).Assembly);

            services.Configure<RouteOptions>(options =>
                options.ConstraintMap.Add("objectId", typeof(ObjectIdRouteConstraint)));
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    opt.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                })
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<GetAuthTokenApiInputValidator>());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}