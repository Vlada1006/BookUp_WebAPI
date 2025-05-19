using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<LocationsCategory> LocCategories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Place> Places { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LocationsCategory>()
            .HasMany(u => u.Locations)
            .WithOne(u => u.LocationsCategory)
            .HasForeignKey(u => u.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Location>()
            .HasMany(u => u.Places)
            .WithOne(u => u.Location)
            .HasForeignKey(u => u.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Place>()
            .HasOne(u => u.Location)
            .WithMany(u => u.Places)
            .HasForeignKey(u => u.LocationId);
        }
    }
}