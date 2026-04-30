using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;
using RestaurantManager.Domain.Entities;
using System.IO;

namespace RestaurantManager.Infrastructure.DbContexts
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RestaurantManagerDbContext>
    {
        public RestaurantManagerDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../RestaurantManager.Api"))
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.MapEnum<DishCategory>("categoria_menu");
            dataSourceBuilder.MapEnum<OrderStatus>("order_status");
            var dataSource = dataSourceBuilder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<RestaurantManagerDbContext>();
            optionsBuilder.UseNpgsql(dataSource);

            return new RestaurantManagerDbContext(optionsBuilder.Options);
        }
    }
}
