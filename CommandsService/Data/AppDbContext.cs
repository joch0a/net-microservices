using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Command> Commands { get; set; }

        public DbSet<Platform> Platforms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder
                .Entity<Platform>()
                .HasMany(platform => platform.Commands)
                .WithOne(command => command.Platform!)
                .HasForeignKey(command => command.PlatformId);

            modelBuilder
                .Entity<Command>()
                .HasOne(command => command.Platform)
                .WithMany(platform => platform.Commands)
                .HasForeignKey(platform => platform.PlatformId);
                
        }
    }
}
