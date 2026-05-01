using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdersAndDishesPermissionsToUsuarioRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // CREAR NUEVOS PERMISOS - Orders y Dishes
            // Usar SQL directo con ON CONFLICT DO NOTHING para que no falle si ya existen
            // ============================================
            migrationBuilder.Sql(@"
                INSERT INTO accesscontrol.""Permissions"" (""Id"", ""Name"", ""Description"", ""Status"", ""CreatedAt"", ""UpdatedAt"")
                VALUES
                    ('a1111111-1111-1111-1111-111111111111', 'orders.read', 'Consultar pedidos', true, NOW(), NULL),
                    ('a2222222-2222-2222-2222-222222222222', 'orders.create', 'Crear pedidos', true, NOW(), NULL),
                    ('a3333333-3333-3333-3333-333333333333', 'orders.update', 'Actualizar pedidos', true, NOW(), NULL),
                    ('a4444444-4444-4444-4444-444444444444', 'dishes.read', 'Consultar platos del menú', true, NOW(), NULL),
                    ('a5555555-5555-5555-5555-555555555555', 'dishes.create', 'Crear platos del menú', true, NOW(), NULL),
                    ('a6666666-6666-6666-6666-666666666666', 'dishes.update', 'Actualizar platos del menú', true, NOW(), NULL),
                    ('a7777777-7777-7777-7777-777777777777', 'dishes.delete', 'Eliminar platos del menú', true, NOW(), NULL)
                ON CONFLICT (""Name"") DO NOTHING;
            ");

            // ============================================
            // ASIGNAR PERMISOS AL ROL USUARIO
            // Usar SQL directo con ON CONFLICT DO NOTHING para que no falle si ya existen
            // ============================================
            migrationBuilder.Sql(@"
                -- Asignar permisos al rol Usuario (eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee)
                INSERT INTO accesscontrol.""RolePermissions"" (""RoleId"", ""PermissionId"")
                VALUES
                    -- Permisos de Orders
                    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'a1111111-1111-1111-1111-111111111111'), -- orders.read
                    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'a2222222-2222-2222-2222-222222222222'), -- orders.create
                    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'a3333333-3333-3333-3333-333333333333'), -- orders.update

                    -- Permisos de Dishes
                    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'a4444444-4444-4444-4444-444444444444'), -- dishes.read
                    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'a5555555-5555-5555-5555-555555555555'), -- dishes.create
                    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'a6666666-6666-6666-6666-666666666666'), -- dishes.update
                    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'a7777777-7777-7777-7777-777777777777'), -- dishes.delete

                    -- Permisos de Reports
                    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '2145dbdc-3e3b-4074-b3d6-c1c64e4f8124')  -- reports.sales
                ON CONFLICT (""RoleId"", ""PermissionId"") DO NOTHING;

                -- Asignar permisos al rol Administrador (bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb)
                INSERT INTO accesscontrol.""RolePermissions"" (""RoleId"", ""PermissionId"")
                VALUES
                    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'a1111111-1111-1111-1111-111111111111'), -- orders.read
                    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'a2222222-2222-2222-2222-222222222222'), -- orders.create
                    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'a3333333-3333-3333-3333-333333333333'), -- orders.update
                    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'a4444444-4444-4444-4444-444444444444'), -- dishes.read
                    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'a5555555-5555-5555-5555-555555555555'), -- dishes.create
                    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'a6666666-6666-6666-6666-666666666666'), -- dishes.update
                    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'a7777777-7777-7777-7777-777777777777')  -- dishes.delete
                ON CONFLICT (""RoleId"", ""PermissionId"") DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar asignaciones de permisos de ambos roles
            migrationBuilder.Sql(@"
                -- Eliminar del rol Usuario
                DELETE FROM accesscontrol.""RolePermissions""
                WHERE ""RoleId"" = 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee'
                AND ""PermissionId"" IN (
                    'a1111111-1111-1111-1111-111111111111',
                    'a2222222-2222-2222-2222-222222222222',
                    'a3333333-3333-3333-3333-333333333333',
                    'a4444444-4444-4444-4444-444444444444',
                    'a5555555-5555-5555-5555-555555555555',
                    'a6666666-6666-6666-6666-666666666666',
                    'a7777777-7777-7777-7777-777777777777',
                    '2145dbdc-3e3b-4074-b3d6-c1c64e4f8124'
                );

                -- Eliminar del rol Administrador
                DELETE FROM accesscontrol.""RolePermissions""
                WHERE ""RoleId"" = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'
                AND ""PermissionId"" IN (
                    'a1111111-1111-1111-1111-111111111111',
                    'a2222222-2222-2222-2222-222222222222',
                    'a3333333-3333-3333-3333-333333333333',
                    'a4444444-4444-4444-4444-444444444444',
                    'a5555555-5555-5555-5555-555555555555',
                    'a6666666-6666-6666-6666-666666666666',
                    'a7777777-7777-7777-7777-777777777777'
                );

                -- Eliminar los permisos creados (no el reports.sales que ya existía)
                DELETE FROM accesscontrol.""Permissions""
                WHERE ""Id"" IN (
                    'a1111111-1111-1111-1111-111111111111',
                    'a2222222-2222-2222-2222-222222222222',
                    'a3333333-3333-3333-3333-333333333333',
                    'a4444444-4444-4444-4444-444444444444',
                    'a5555555-5555-5555-5555-555555555555',
                    'a6666666-6666-6666-6666-666666666666',
                    'a7777777-7777-7777-7777-777777777777'
                );
            ");
        }
    }
}
