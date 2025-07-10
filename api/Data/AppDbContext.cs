using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<CategoryForLocations> LocCategories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CategoryForLocations>()
            .HasMany(u => u.Locations)
            .WithOne(u => u.CategoryForLocations)
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

            builder.Entity<Booking>()
            .HasOne(u => u.Place)
            .WithMany(u => u.Bookings)
            .HasForeignKey(u => u.PlaceId)
            .OnDelete(DeleteBehavior.Cascade);


            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = "1",
                    Name ="Admin",
                    NormalizedName="ADMIN"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}