using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CQRS
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddAplicationServices(
            this IServiceCollection services)
        {
            //Registrar nuestros manejadores, es lo que hace el IoC
            services.AddMediatR(Assembly.GetExecutingAssembly());
                return services;
        }
    }
}
