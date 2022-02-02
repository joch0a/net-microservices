using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;
using System;
using System.Linq;

namespace PlatformService.Data
{
    // Testing only, not for production.

    public static class PrepareDb
    {
        public static void PreparePopulation(IApplicationBuilder app, bool isProd)
        {
            using var servicesScope = app.ApplicationServices.CreateScope();

            SeedData(servicesScope.ServiceProvider.GetService<AppDbContext>(), isProd);
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempint to run migrations");

                try
                {
                    context.Database.Migrate();
                }
                catch(Exception exc) 
                {
                    Console.WriteLine($"--> Could not run migrations: {exc.Message}");
                }
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data...");

                context.Platforms.AddRange(
                    new Platform()
                    {
                        Name = "Dot net",
                        Publisher = "Microsoft",
                        Cost = "Free"
                    },
                    new Platform()
                    {
                        Name = "SQL Server Express",
                        Publisher = "Microsoft",
                        Cost = "Free"
                    },
                    new Platform()
                    {
                        Name = "Kubernetes",
                        Publisher = "Cloud Native Computing Foundation",
                        Cost = "Free"
                    }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}
