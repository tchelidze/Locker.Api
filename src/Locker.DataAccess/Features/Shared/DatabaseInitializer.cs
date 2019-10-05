using System;
using System.Linq;
using System.Threading.Tasks;
using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Auth.Repositories;
using Locker.Domain.Features.LockManagement.Entities;
using Locker.Domain.Features.LockManagement.Repositories;

namespace Locker.DataAccess.Features.Shared
{
    public class DatabaseInitializer
    {
        private readonly IUserRepository _userRepository;
        private readonly ILockRepository _lockRepository;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseInitializer(
            IUserRepository userRepository,
            IServiceProvider serviceProvider,
            ILockRepository lockRepository)
        {
            _userRepository = userRepository;
            _serviceProvider = serviceProvider;
            _lockRepository = lockRepository;
        }

        public Task ConfigureDbCollections()
        {
            return Task.WhenAll(
                typeof(BaseMongoRepository<>)
                    .Assembly
                    .GetTypes()
                    .Where(type => typeof(IConfigureCollection).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    .Select(type =>
                    {
                        var service = _serviceProvider.GetService(type);
                        if (service != null)
                        {
                            return service;
                        }

                        var serviceInterface = type.GetInterface($"I{type.Name}");

                        if (serviceInterface == null)
                        {
                            return null;
                        }

                        return _serviceProvider.GetService(serviceInterface);
                    })
                    .Where(repo => repo != null)
                    .Select(repo => ((IConfigureCollection)repo).ConfigureCollection()));
        }

        public Task Seed() => Task.WhenAll(SeedUsers(), SeedLocks());

        private Task SeedUsers()
        {
            var adminUser = new User
            {
                UserName = "admin",
                Permissions = new[]
                {
                    new UserPermission
                    {
                        AccessRight = ResourceAccessRight.All,
                        Resource = "Users"
                    },
                    new UserPermission
                    {
                        AccessRight = ResourceAccessRight.All,
                        Resource = "Locks"
                    }
                }
            };

            adminUser.SetPassword("1234DSAT1234");

            return _userRepository.TryAddUser(adminUser);
        }

        private async Task SeedLocks()
        {
            await _lockRepository.TryAdd(new Lock
            {
                FriendlyName = "Tunnel Door Lock"
            }).ConfigureAwait(false);

            await _lockRepository.TryAdd(new Lock
            {
                FriendlyName = "Office Gate Door Lock"
            }).ConfigureAwait(false);
        }
    }
}
