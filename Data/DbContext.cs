using ImmageAggregatorAPI.Entities;
using ImmageAggregatorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ImmageAggregatorAPI
{
    public class IAAPIDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public IAAPIDbContext(DbContextOptions<IAAPIDbContext> options) : base(options)
        {
        }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLocationMap> UserLocationMaps { get; set; }
        public DbSet<ImageBlob> ImageBlobs { get; set; }
        public DbSet<ImageLocationMap> ImageLocationMaps { get; set; }

    }
}
