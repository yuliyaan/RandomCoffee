using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RandomCoffee.EntityFramework.Models;

namespace RandomCoffee.EntityFramework
{
    /// <summary>
    /// Random coffee database context
    /// </summary>
    public class Db : DbContext
    {
        private readonly IConfiguration _configuration;
        
        public DbSet<RandomCoffeeRecord> RandomCoffeeRecords { get; set; }
        
        public Db(IConfiguration configuration)
        {
            _configuration = configuration;
            
            Database.Migrate(); // Enable automatic migration on app's start
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}