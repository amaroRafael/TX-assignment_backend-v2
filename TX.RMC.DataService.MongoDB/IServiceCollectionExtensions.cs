﻿namespace TX.RMC.DataService.MongoDB;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TX.RMC.DataAccess.Core.Contracts;
using TX.RMC.DataService.MongoDB.Repositories;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDbServices(this IServiceCollection services, string connectionString, string databaseName)
    {
        var mongoDBoptions = new Options.MongoDBOptions
        {
            ConnectionString = connectionString,
            DatabaseName = databaseName,
        };
        services.AddSingleton(mongoDBoptions);

#if DEBUG
        services.AddLogging(o =>
        {
            o.AddConsole();
            o.AddDebug();
        });

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
            .AddConsole()
            .AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name
                && level == LogLevel.Information);
        });
#endif
        services.AddDbContext<MongoDBContext>(
            options =>
            {
                options.UseMongoDB(mongoDBoptions.ConnectionString, mongoDBoptions.DatabaseName);
#if DEBUG
                options.UseLoggerFactory(loggerFactory);
#endif
            });

        services.AddScoped<IUserDataRepository, UserDataRepository>();
        services.AddScoped<IRobotDataRepository, RobotDataRepository>();
        services.AddScoped<ICommandDataRepository, CommandDataRepository>();

        return services;
    }
}
