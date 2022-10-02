using Caravel.ApplicationContext;
using Caravel.AspNetCore.Authentication;
using Caravel.Clock;
using Caravel.MediatR.Behaviours;
using Caravel.MediatR.Security;
using CaravelTemplate.Core.Books.Queries;
using CaravelTemplate.Identity;
using CaravelTemplate.Identity.Authentication;
using CaravelTemplate.Infrastructure.Data.Repositories;
using CaravelTemplate.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CaravelTemplate.WebApi.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            var coreAssembly = typeof(GetBookByIdQuery).Assembly;
            var identityAssembly = typeof(IdentityService).Assembly;
            
            services.AddFluentValidationAutoValidation().AddValidatorsFromAssembly(coreAssembly);
            services.AddAutoMapper(coreAssembly);
            services.AddMediatR(coreAssembly, identityAssembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            
            services.AddHttpContextAccessor();
            services.AddScoped<IClock, DateTimeUtcClock>();
            services.AddScoped<IAppContextAccessor, AppContextAccessor>();
            services.AddScoped<ITokenFactory, TokenFactory>();
            services.AddScoped<IJwtManager, JwtManager>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAuthorizer, IdentityService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBookRepository, BookRepository>();
            
        }
    }
}