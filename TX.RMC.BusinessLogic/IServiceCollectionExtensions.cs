namespace TX.RMC.BusinessLogic;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
    {
        services.AddSingleton<CommandService>();
        services.AddSingleton<HistoryService>();
        services.AddSingleton<StatusService>();
        services.AddSingleton<LoginService>();

        return services;
    }
}
