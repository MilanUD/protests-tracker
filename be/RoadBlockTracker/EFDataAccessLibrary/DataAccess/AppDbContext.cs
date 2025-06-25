using EFDataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccessLibrary.DataAccess
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Protest> Protests { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Protest>().OwnsMany(p => p.Locations, locationBuilder =>
            {
                locationBuilder.WithOwner();

                locationBuilder.Property(l => l.Address).HasMaxLength(200);
                locationBuilder.Property(l => l.City).HasMaxLength(100);
                locationBuilder.Property(l => l.Country).HasMaxLength(100);
                locationBuilder.Property(l => l.Latitude);
                locationBuilder.Property(l => l.Longitude);
                locationBuilder.Property(l => l.AdditionalInfo).HasMaxLength(500);

            });
        }


    }
}
