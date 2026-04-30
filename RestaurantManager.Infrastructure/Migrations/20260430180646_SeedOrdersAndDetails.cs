using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedOrdersAndDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // DATOS INICIALES - orders
            // ============================================
            migrationBuilder.Sql(@"
                INSERT INTO public.orders (id, table_number, waiter_name, order_date, status) VALUES
                (1, 5, 'Juan Perez', NOW() - INTERVAL '2 hours', 'Entregado'),
                (2, 3, 'Maria Lopez', NOW() - INTERVAL '1 hour', 'EnPreparacion'),
                (3, 8, 'Carlos Ruiz', NOW() - INTERVAL '30 minutes', 'Abierto');
                
                -- Reiniciar el contador de la secuencia para orders
                SELECT setval(pg_get_serial_sequence('public.orders', 'id'), (SELECT MAX(id) FROM public.orders));
            ");

            // ============================================
            // DATOS INICIALES - order_details
            // ============================================
            migrationBuilder.Sql(@"
                INSERT INTO public.order_details (id, order_id, menu_item_id, quantity, unit_price, notes) VALUES
                (1, 1, 6, 1, 28000, 'Sin cebolla'),
                (2, 1, 16, 1, 7000, ''),
                (3, 2, 7, 2, 24000, 'Bien caliente'),
                (4, 3, 1, 1, 12000, 'Con extra ají');
                
                -- Reiniciar el contador de la secuencia para order_details
                SELECT setval(pg_get_serial_sequence('public.order_details', 'id'), (SELECT MAX(id) FROM public.order_details));
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM public.order_details WHERE id IN (1, 2, 3, 4);");
            migrationBuilder.Sql("DELETE FROM public.orders WHERE id IN (1, 2, 3);");
        }
    }
}
