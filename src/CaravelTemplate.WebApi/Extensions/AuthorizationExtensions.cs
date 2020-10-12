using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caravel.MediatR.Behaviours;
using Microsoft.Extensions.DependencyInjection;

namespace CaravelTemplate.WebApi.Extensions
{
    public static class AuthorizationExtensions
    {
        public static void AddAuthorizeFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            assembly.GetTypesAssignableTo(typeof(IAuthorize<>))
                ?.ForEach((type) =>
            {
                foreach (var implementedInterface in type.ImplementedInterfaces)
                {
                    switch (lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddScoped(implementedInterface, type);
                            break;
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(implementedInterface, type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(implementedInterface, type);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
                    }
                }
            });
        }

        private static List<TypeInfo>? GetTypesAssignableTo(this Assembly assembly, Type compareType)
        {
            var typeInfoList = assembly.DefinedTypes.Where(x => x.IsClass
                                                                && !x.IsAbstract
                                                                && x != compareType
                                                                && x.GetInterfaces()
                                                                    .Any(i => i.IsGenericType
                                                                              && i.GetGenericTypeDefinition() ==
                                                                              compareType))?.ToList();

            return typeInfoList;
        }
    }
}