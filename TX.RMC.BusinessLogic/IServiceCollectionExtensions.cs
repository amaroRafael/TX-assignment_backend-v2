﻿namespace TX.RMC.BusinessLogic;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Register Business Logic Services in the Service Collection
    /// </summary>
    public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
    {
        services.AddTransient<CommandService>();
        services.AddTransient<RobotService>();
        services.AddTransient<UserService>();

        return services;
    }
}
