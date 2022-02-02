using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;
using System;

namespace PlatformService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _env;

        public void ConfigureServices(IServiceCollection services)
        {
            AddDatabaseConfiguration(services);

            services.AddScoped<IPlatformRepository, PlatformRepository>();
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });

            Console.WriteLine($"--> CommandService endpoint: {Configuration["CommandService"]}");
        }

        private void AddDatabaseConfiguration(IServiceCollection services)
        {
            if (_env.IsDevelopment())
            {
                Console.WriteLine("--> Using In memory");

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMem");
                });
            }
            else
            {
                Console.WriteLine("--> Using SQL Server");

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("PlatformsConn"));
                });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            PrepareDb.PreparePopulation(app, env.IsProduction());
        }
    }
}
