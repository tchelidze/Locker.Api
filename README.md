# Least Magic ASP.NET Core 3.0 Starter Rest Web Api Implementation

The idea behind the application is to use least general-purpose libraries which can be replaced with clear, understandable and manageable few lines of code

Included/demonstrated features

1. Token Based Authentication, supporting `password` grant type, implemented **manually** without back boxes such [OpenIdDict](https://www.nuget.org/packages/OpenIddict/) or [IdentityServer](https://www.nuget.org/packages/IdentityServer4/) and 1K lines of configuration
2. Identity Management implemented **manually** without [Microsoft.AspNetCore.Identity](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity/) and familiar `AspNet..` tables/collections
3. Fine-grained Authorization implemented [manually](https://github.com/tchelidze/Locker.Api/blob/master/src/Locker.Api/Web/Filters/CrudApiFilterAttribute.cs)
4. [Route Constraints](https://github.com/tchelidze/Locker.Api/blob/master/src/Locker.Api/Web/RouteConstraints/ObjectIdRouteConstraint.cs) for crosscutting route data validation
5. [Model State Validator]() Action Filter for crosscutting model state validation (`if(!ModelState.IsValid) { ... })` basically)
6. [Screaming Architecture](https://blog.cleancoder.com/uncle-bob/2011/09/30/Screaming-Architecture.html) for organizing the folder structure
7. [MediatR](https://github.com/jbogard/MediatR) as mediator between API layer and the domain
8. MongoDb as a persistance mechanism (surprisingly)
9. [Unit Tests](https://github.com/tchelidze/Locker.Api/tree/master/test/Locker.UnitTests) for all main parts of the App 

Repository includes [Postman collection](https://github.com/tchelidze/Locker.Api/blob/master/docs/Locker.Api.postman_collection.json) to make api exploration easier

