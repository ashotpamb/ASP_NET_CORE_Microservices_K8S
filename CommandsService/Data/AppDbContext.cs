using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder 
                .Entity<Platform>()
                .HasMany(p => p.Commands)
                .WithOne(c =>  c.Platform)
                .HasForeignKey(p => p.PlatfromId);
            modelBuilder
                .Entity<Command>()
                .HasOne(p => p.Platform)
                .WithMany(c => c.Commands)
                .HasForeignKey(p => p.PlatfromId);
        }
    }



}