using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;
using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Infrastructure.DbContexts
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RestaurantManagerDbContext>
    {
        public RestaurantManagerDbContext CreateDbContext(string[] args)
        {
            // Crear DataSource con mapeo de enums
            var connectionString = "Host=localhost;Database=restaurant_manager;Username=postgres;Password=admin";
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
