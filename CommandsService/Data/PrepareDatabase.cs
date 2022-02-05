using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace CommandsService.Data
{
    public static class PrepareDatabase
    {
        public static void PreparePopulation(IApplicationBuilder applicationBuilder) 
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
            var commandRepository = serviceScope.ServiceProvider.GetService<ICommandRepository>();
            var platforms = grpcClient.ReturnAllPlatforms();

            SeedData(commandRepository, platforms);
        }

        private static void SeedData(ICommandRepository commandRepository, IEnumerable<Platform> platforms) 
        {
            Console.WriteLine("--> Seeding new platforms...");

            foreach (var platform in platforms)
            {
                if (!commandRepository.ExternalPlatformExists(platform.ExternalId))
                {
                    commandRepository.CreatePlatform(platform);
                }
            }

            commandRepository.SaveChanges();
        }
    }
}
