using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace RaceDay.API.Data
{
    public class RaceDayDbContextFactory : IDesignTimeDbContextFactory<RaceDayDbContext>
    {
        public RaceDayDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RaceDayDbContext>();

            // Load the connection string from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new RaceDayDbContext(optionsBuilder.Options);
        }
    }
}