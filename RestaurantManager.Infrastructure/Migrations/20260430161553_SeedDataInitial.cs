using Microsoft.EntityFrameworkCore.Migrations;
using RestaurantManager.Domain.Entities;

#nullable disable

namespace RestaurantManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // DATOS INICIALES - menu_items (platos)
            // ============================================
            migrationBuilder.Sql(@"
                INSERT INTO public.menu_items (id, name, description, price, category, is_available, created_at, updated_at) VALUES
                (1, 'Empanadas de carne', 'Empanadas criollas con ají casero (3 unidades)', 12000, 'Entradas', true, NOW(), NOW()),
                (2, 'Patacones con hogao', 'Patacones verdes con hogao de tomate y cebolla', 10000, 'Entradas', true, NOW(), NOW()),
                (3, 'Arepa de choclo con queso', 'Arepa dulce de maíz tierno con queso campesino', 8000, 'Entradas', true, NOW(), NOW()),
                (4, 'Chicharrón con limón', 'Chicharrón crocante con limón y ají', 15000, 'Entradas', true, NOW(), NOW()),
                (5, 'Ceviche de camarón', 'Ceviche fresco con camarón, limón y cilantro', 18000, 'Entradas', false, NOW(), NOW()),
                (6, 'Bandeja Paisa', 'Frijoles, arroz, carne molida, chicharrón, huevo, plátano, arepa, aguacate and chorizo', 28000, 'PlatosFuertes', true, NOW(), NOW()),
                (7, 'Ajiaco Santafereño', 'Sopa de tres papas con pollo, mazorca, guascas, crema y alcaparras', 24000, 'PlatosFuertes', true, NOW(), NOW()),
                (8, 'Pescado frito con patacón', 'Mojarra frita con arroz de coco, patacón y ensalada', 26000, 'PlatosFuertes', true, NOW(), NOW()),
                (9, 'Lomo de cerdo en salsa BBQ', 'Lomo de cerdo a la parrilla con salsa BBQ casera, papas criollas y ensalada', 30000, 'PlatosFuertes', true, NOW(), NOW()),
                (10, 'Pollo a la plancha', 'Pechuga a la plancha con arroz, ensalada y papas a la francesa', 22000, 'PlatosFuertes', true, NOW(), NOW()),
                (11, 'Cazuela de mariscos', 'Cazuela de mariscos con arroz de coco', 35000, 'PlatosFuertes', true, NOW(), NOW()),
                (12, 'Trucha al ajillo', 'Trucha fresca al ajillo con vegetales salteados y arroz', 27000, 'PlatosFuertes', false, NOW(), NOW()),
                (13, 'Sancocho de gallina', 'Sancocho tradicional con gallina criolla, yuca, plátano y mazorca', 20000, 'Sopas', true, NOW(), NOW()),
                (14, 'Sopa de tortilla', 'Sopa cremosa de tortilla con trozos de pollo y aguacate', 16000, 'Sopas', true, NOW(), NOW()),
                (15, 'Mondongo', 'Mondongo tradicional con papa, yuca y cilantro', 18000, 'Sopas', true, NOW(), NOW()),
                (16, 'Limonada de coco', 'Limonada natural con leche de coco', 7000, 'Bebidas', true, NOW(), NOW()),
                (17, 'Jugo de lulo', 'Jugo natural de lulo en agua', 6000, 'Bebidas', true, NOW(), NOW()),
                (18, 'Agua de panela con limón', 'Agua de panela fría con rodajas de limón', 5000, 'Bebidas', true, NOW(), NOW()),
                (19, 'Cerveza Club Colombia', 'Cerveza Club Colombia dorada 330ml', 8000, 'Bebidas', true, NOW(), NOW()),
                (20, 'Gaseosa', 'Coca-Cola, Sprite o Colombiana 350ml', 4000, 'Bebidas', true, NOW(), NOW()),
                (21, 'Tres leches', 'Pastel de tres leches con canela', 12000, 'Postres', true, NOW(), NOW()),
                (22, 'Arroz con leche', 'Arroz con leche casero con canela y pasas', 8000, 'Postres', true, NOW(), NOW()),
                (23, 'Natilla con buñuelos', 'Natilla tradicional con buñuelos (3 unidades)', 10000, 'Postres', true, NOW(), NOW()),
                (24, 'Helado de guanábana', 'Helado artesanal de guanábana (2 bolas)', 9000, 'Postres', false, NOW(), NOW()),
                (25, 'Pastas al burro', 'Es una receta italiana tradicional que consiste en pasta salteada simplemente con mantequilla y queso parmesano.', 20000, 'Sopas', true, NOW(), NOW());
            ");

            // ============================================
            // DATOS INICIALES - UserTypes
            // ============================================
            migrationBuilder.InsertData(
                table: "UserTypes",
                schema: "accesscontrol",
                columns: new[] { "Id", "Name", "Description", "Status", "CreatedAt", "UpdatedAt", "AdditionalConfig" },
                values: new object[,]
                {
                    { 
                        new Guid("82d97e80-eba4-45ea-9f41-0d85e0a7413d"), 
                        "Administrador", 
                        "Usuarios con control total sobre la aplicación.", 
                        true, 
                        DateTime.UtcNow, 
                        null,
                        @"
                        {
                          ""navigation"": [
                            {
                              ""menuId"": ""ac8a0b8c-7125-4a1c-9be5-0a875f840451"",
                              ""label"": ""Dashboard"",
                              ""icon"": ""fas fa-tachometer-alt"",
                              ""route"": ""/dashboard"",
                              ""parentId"": """",
                              ""permissions"": ""read,create,edit,delete"",
                              ""children"": []
                            },
                            {
                              ""menuId"": ""ddf6af91-aac5-43c2-923c-4ec0b5b288d6"",
                              ""label"": ""Autenticación"",
                              ""icon"": ""shield"",
                              ""route"": ""/Auth"",
                              ""permissions"": ""read,create,edit,delete"",
                              ""children"": [
                                {
                                  ""menuId"": ""88dab63f-1cb1-4908-a067-b3c7fa8bb614"",
                                  ""label"": ""Usuarios"",
                                  ""icon"": ""people"",
                                  ""route"": ""/auth/users"",
                                  ""parentId"": ""ddf6af91-aac5-43c2-923c-4ec0b5b288d6"",
                                  ""permissions"": ""read,create,edit,delete""
                                },
                                {
                                  ""menuId"": ""c1ad6944-862c-4feb-8cab-f7ad411a9623"",
                                  ""label"": ""Roles"",
                                  ""icon"": ""AssignmentInd"",
                                  ""route"": ""/auth/roles"",
                                  ""parentId"": ""ddf6af91-aac5-43c2-923c-4ec0b5b288d6"",
                                  ""permissions"": ""read,create,edit,delete""
                                },
                                {
                                  ""menuId"": ""0a15d5a2-626a-4209-92e9-34f63cef4a0f"",
                                  ""label"": ""Permisos"",
                                  ""icon"": ""VerifiedUser"",
                                  ""route"": ""/auth/permissions"",
                                  ""parentId"": ""ddf6af91-aac5-43c2-923c-4ec0b5b288d6"",
                                  ""permissions"": ""read,create,edit,delete""
                                },
                                {
                                  ""menuId"": ""a7d1eb00-712c-4e63-8c83-83a0c71cfce9"",
                                  ""label"": ""Tipos de usuarios"",
                                  ""icon"": ""GroupAdd"",
                                  ""route"": ""/auth/user-types"",
                                  ""parentId"": ""ddf6af91-aac5-43c2-923c-4ec0b5b288d6"",
                                  ""permissions"": ""read,create,edit,delete""
                                }
                              ]
                            }
                          ]
                        }"
                    },
                    { 
                        new Guid("d13d0d11-acc7-4c8f-8cb9-e6b2706cc89a"), 
                        "Usuario", 
                        "Usuario del sistema", 
                        true, 
                        DateTime.UtcNow, 
                        null,
                        @"
                        {
                          ""navigation"": [
                            {
                              ""menuId"": ""ac8a0b8c-7125-4a1c-9be5-0a875f840451"",
                              ""label"": ""Dashboard"",
                              ""icon"": ""fas fa-tachometer-alt"",
                              ""route"": ""/dashboard"",
                              ""parentId"": """",
                              ""permissions"": ""read,create,edit,delete"",
                              ""children"": []
                            },
                            {
                              ""menuId"": ""2b1ca67f-fb93-4e29-b613-96044636133f"",
                              ""label"": ""Restaurante"",
                              ""icon"": ""Restaurant"",
                              ""route"": ""/restaurant"",
                              ""parentId"": """",
                              ""permissions"": ""read,create,edit,delete"",
                              ""children"": [
                                {
                                  ""menuId"": ""2e348424-8184-4e7b-aaff-c88b44f0df13"",
                                  ""label"": ""Menu"",
                                  ""icon"": ""MenuBook"",
                                  ""route"": ""/restaurant/dishes"",
                                  ""parentId"": ""2b1ca67f-fb93-4e29-b613-96044636133f"",
                                  ""permissions"": ""read,create,edit,delete"",
                                  ""children"": []
                                },
                                {
                                  ""menuId"": ""95dab6c5-b459-449d-9d88-cb7dbee78f04"",
                                  ""label"": ""Pedidos"",
                                  ""icon"": ""DinnerDining"",
                                  ""route"": ""/restaurant/orders"",
                                  ""parentId"": ""2b1ca67f-fb93-4e29-b613-96044636133f"",
                                  ""permissions"": ""read,create,edit,delete"",
                                  ""children"": []
                                }
                              ]
                            },
                            {
                              ""menuId"": ""d036385a-ac9f-4e02-a7d1-a6f6c7148bce"",
                              ""label"": ""Informes y Analíticas"",
                              ""icon"": ""Analytics"",
                              ""route"": ""/report"",
                              ""parentId"": """",
                              ""permissions"": ""read,create,edit,delete"",
                              ""children"": [
                                {
                                  ""menuId"": ""ac8a0b8c-7125-4a1c-9be5-0a875f840444"",
                                  ""label"": ""Reportes"",
                                  ""icon"": ""BarChart"",
                                  ""route"": ""/restaurant/reports"",
                                  ""parentId"": ""d036385a-ac9f-4e02-a7d1-a6f6c7148bce"",
                                  ""permissions"": ""read,create,edit,delete"",
                                  ""children"": []
                                }
                              ]
                            }
                          ]
                        }"
                    }
                });

            // ============================================
            // DATOS INICIALES - Permissions
            // ============================================
            migrationBuilder.InsertData(
                table: "Permissions",
                schema: "accesscontrol",
                columns: new[] { "Id", "Name", "Description", "Status", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00e2ee71-5816-4ce6-aefd-cda8b1f10d0f"), "suppliers.create", "Crear proveedores", true, DateTime.UtcNow, null },
                    { new Guid("030640f8-5b1c-4a44-ac0a-7702a4e63008"), "user_types.update", "Actualizar tipos de usuario", true, DateTime.UtcNow, null },
                    { new Guid("0c90e6be-52f5-4378-b6d5-a3c5b1b5cdb6"), "menu.delete", "Permiso para eliminar menu", true, DateTime.UtcNow, null },
                    { new Guid("0f4de161-4aa0-4e78-bbe5-7d083cddd604"), "permissions.read", "Consultar permisos", true, DateTime.UtcNow, null },
                    { new Guid("16156551-e75f-441a-b279-97c5419fbc1e"), "clients.delete", "Eliminar clientes", true, DateTime.UtcNow, null },
                    { new Guid("2145dbdc-3e3b-4074-b3d6-c1c64e4f8124"), "reports.sales", "Generar reportes de ventas", true, DateTime.UtcNow, null },
                    { new Guid("24ef1acc-9a55-4b86-9868-a7fe2d01ca6f"), "suppliers.read", "Consultar proveedores", true, DateTime.UtcNow, null },
                    { new Guid("2660d239-eafc-490b-8cb0-03e39120a244"), "projects.edit", "Permiso para editar proyectos", true, DateTime.UtcNow, null },
                    { new Guid("282da9b9-ba97-4d38-a267-2339daeb3957"), "users.manage_additional_data", "Gestionar datos adicionales de usuarios", true, DateTime.UtcNow, null },
                    { new Guid("286989b0-fe3e-42dc-862e-8cfb9a6c47a8"), "projects.read", "Permiso para leer proyectos", true, DateTime.UtcNow, null },
                    { new Guid("2c85aa39-60b0-41e8-9d60-1e12c37918fd"), "reviews.delete", "Permiso para borrar reseña", true, DateTime.UtcNow, null },
                    { new Guid("355c217f-3ab3-4f82-a753-8a339610a27d"), "schedules.read", "Permiso para leer agendas", true, DateTime.UtcNow, null },
                    { new Guid("4792f730-b1ca-4af2-a281-8a97250ffefb"), "suppliers.update", "Actualizar proveedores", true, DateTime.UtcNow, null },
                    { new Guid("4d1e7d33-3470-4d6d-8d6e-e8227b73a3fd"), "schedules.delete", "Permiso para eliminar agendas", true, DateTime.UtcNow, null },
                    { new Guid("4e2f78f0-6d4c-4e65-9029-efa4dbb519b1"), "permissions.create", "Crear permisos", true, DateTime.UtcNow, null },
                    { new Guid("4f134eb3-2dc0-4806-8519-41cb7727efc6"), "clients.read", "Consultar información de clientes", true, DateTime.UtcNow, null },
                    { new Guid("501cadb6-4f7c-43b8-b2bb-3463ee2d5440"), "reviews.create", "Permiso para crear reseña", true, DateTime.UtcNow, null },
                    { new Guid("5412ed73-19a3-487a-bcfd-beae36c5a27e"), "permissions.delete", "Eliminar permisos", true, DateTime.UtcNow, null },
                    { new Guid("5449c302-93c8-4670-ba28-0567dba694ba"), "user_types.read", "Consultar tipos de usuario", true, DateTime.UtcNow, null },
                    { new Guid("5c2cb257-2d18-4870-9675-da43b772a5cb"), "reports.users", "Generar reportes de usuarios", true, DateTime.UtcNow, null },
                    { new Guid("61731b86-0223-44f5-80be-eb8e9dd8327e"), "roles.update", "Actualizar roles", true, DateTime.UtcNow, null },
                    { new Guid("6d1c2462-fa3e-4a68-86e8-b6896cdaccd6"), "projects.create", "Permiso para crear proyectos", true, DateTime.UtcNow, null },
                    { new Guid("6e2281c1-e626-4daf-b5ec-7da12deaa6a5"), "analytics.dashboard", "Acceder al dashboard de analytics", true, DateTime.UtcNow, null },
                    { new Guid("73cb2962-aef2-42a4-8407-cf0869625ede"), "suppliers.delete", "Eliminar proveedores", true, DateTime.UtcNow, null },
                    { new Guid("76aeb57f-cdf2-40db-b02e-cc175e7253c0"), "clients.create", "Crear clientes", true, DateTime.UtcNow, null },
                    { new Guid("80c6834a-7ddf-4232-987d-638196cb3972"), "reports.purchases", "Generar reportes de compras", true, DateTime.UtcNow, null },
                    { new Guid("81addf88-c92f-4d4a-90e2-1969e80a4551"), "users.delete", "Eliminar usuarios", true, DateTime.UtcNow, null },
                    { new Guid("8ac8e2be-83ae-4bbf-a599-91dab83cd1f7"), "roles.assign_permissions", "Asignar permisos a roles", true, DateTime.UtcNow, null },
                    { new Guid("8e395540-fde7-4f2c-a360-62a9acb08c39"), "clients.update", "Actualizar información de clientes", true, DateTime.UtcNow, null },
                    { new Guid("94737ae8-877a-4d01-99ea-8b9f69154f56"), "users.change_password", "Cambiar contraseñas de usuarios", true, DateTime.UtcNow, null },
                    { new Guid("962f2a3c-5891-4836-b556-dbe287d17876"), "permissions.update", "Actualizar permisos", true, DateTime.UtcNow, null },
                    { new Guid("a01ec000-518f-4fec-a8b5-30af492eedc9"), "projects.delete", "Permiso para eliminar proyectos", true, DateTime.UtcNow, null },
                    { new Guid("a04e82f0-1cb1-4fd3-a3a3-8d945ab90724"), "reports.inventory", "Generar reportes de inventario", true, DateTime.UtcNow, null },
                    { new Guid("a15c1b24-b3ac-44d4-87c7-b645b54bd32c"), "roles.manage", "Permiso para asignar multiples permisos a un rol", true, DateTime.UtcNow, null },
                    { new Guid("a6e82580-5cef-48aa-acad-edcaa825ade6"), "roles.create", "Crear roles", true, DateTime.UtcNow, null },
                    { new Guid("ab8b6e77-fc91-49a2-9800-a859a1e6c648"), "services.edit", "Permiso para editar servicios", true, DateTime.UtcNow, null },
                    { new Guid("af015994-520b-4e03-abb0-cf0cf9975675"), "roles.delete", "Eliminar roles", true, DateTime.UtcNow, null },
                    { new Guid("b2a3f726-dcf7-4073-ba61-ecd5dd09dd92"), "services.read", "Permiso para leer servicios", true, DateTime.UtcNow, null },
                    { new Guid("b3a75e58-e3c2-4519-9cae-ac0653ac0eb3"), "user_types.create", "Crear tipos de usuario", true, DateTime.UtcNow, null },
                    { new Guid("b47fac30-82cb-46da-85b8-b1046ae6916d"), "suppliers.view_by_name", "Buscar proveedores por nombre", true, DateTime.UtcNow, null },
                    { new Guid("bba7db9f-4562-4849-b620-7a47f100f4cb"), "schedules.edit", "Permiso para editar agendas", true, DateTime.UtcNow, null },
                    { new Guid("bc6c8488-06c8-4246-8253-a80ac8b5625d"), "user_types.delete", "Eliminar tipos de usuario", true, DateTime.UtcNow, null },
                    { new Guid("c236764a-03a2-428a-b1e2-3819dbd2bfa6"), "reviews.read", "Permiso para leer reseñas", true, DateTime.UtcNow, null },
                    { new Guid("c5f88de6-304a-44c2-a28d-178d33d49152"), "menu.update", "Permiso para actualizar menu", true, DateTime.UtcNow, null },
                    { new Guid("c8354179-dc42-45b7-a1b8-34b7b1073f87"), "services.delete", "Permiso para eliminar servicios", true, DateTime.UtcNow, null },
                    { new Guid("c9fe2949-b248-4c13-9e3e-aba40a21b1d9"), "reports.financial", "Generar reportes financieros", true, DateTime.UtcNow, null },
                    { new Guid("cc0d0fde-68d4-44c0-8c00-e7ee24501413"), "menu.read", "Permiso para leer menu", true, DateTime.UtcNow, null },
                    { new Guid("d175300f-22c8-487e-95eb-54f32fc1b5b5"), "reviews.edit", "Permiso para editar reseñas", true, DateTime.UtcNow, null },
                    { new Guid("d8e9507a-5b12-4d2a-8c98-1a52b1466a93"), "users.read", "Consultar información de usuarios", true, DateTime.UtcNow, null },
                    { new Guid("dc82f99d-6f48-4fe7-b3f4-49260b8ac9d8"), "services.create", "Permiso para crear servicios", true, DateTime.UtcNow, null },
                    { new Guid("e1d93604-6a4b-4468-ae77-00ea465042a0"), "users.update", "Actualizar información de usuarios", true, DateTime.UtcNow, null },
                    { new Guid("f0b694ec-335c-4f3d-b4f3-58e33e423737"), "menu.create", "Permiso para crear menu", true, DateTime.UtcNow, null },
                    { new Guid("f12b05f9-c256-4dcd-bcfa-1b791e4cdc82"), "roles.read", "Consultar roles", true, DateTime.UtcNow, null },
                    { new Guid("f5054155-11c5-4077-ac9a-4d02733e4ffe"), "schedules.create", "Permiso para crear reuniones", true, DateTime.UtcNow, null },
                    { new Guid("f7c3c0a4-2b6d-4e92-9a0d-6e1d948a43f8"), "users.create", "Crear nuevos usuarios", true, DateTime.UtcNow, null }
                });

            // ============================================
            // DATOS INICIALES - Roles
            // ============================================
            migrationBuilder.InsertData(
                table: "Roles",
                schema: "accesscontrol",
                columns: new[] { "Id", "Name", "Description", "Status", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Administrador", "Gestión y configuración completa del sistema.", true, DateTime.UtcNow, null },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Usuario", "Acceso y gestión de la información del restaurante", true, DateTime.UtcNow, null }
                });

            // ============================================
            // DATOS INICIALES - Users
            // ============================================
            migrationBuilder.InsertData(
                table: "Users",
                schema: "accesscontrol",
                columns: new[] { "Id", "Name", "Email", "UserTypeId", "Status", "CreatedAt", "UpdatedAt", "Password", "ExtraData" },
                values: new object[,]
                {
                    { new Guid("57a98f97-9271-4aff-83d4-09332cfacb31"), "Administrador", "admin@gmail.com", new Guid("82d97e80-eba4-45ea-9f41-0d85e0a7413d"), true, DateTime.UtcNow, null, "$2a$12$yYX4KKLz5QIUB0Ee/mNG3eK6iwdfg/Vn3OWx7n7EQmO1hLlLIOhE2", "{}" },
                    { new Guid("383d5956-1326-4263-b6a6-6d8bf9be4728"), "Usuario", "usuario@gmail.com", new Guid("d13d0d11-acc7-4c8f-8cb9-e6b2706cc89a"), true, DateTime.UtcNow, null, "$2a$12$yYX4KKLz5QIUB0Ee/mNG3eK6iwdfg/Vn3OWx7n7EQmO1hLlLIOhE2", "{}" }
                });

            // ============================================
            // DATOS INICIALES - UserRoles
            // ============================================
            migrationBuilder.InsertData(
                table: "UserRoles",
                schema: "accesscontrol",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("57a98f97-9271-4aff-83d4-09332cfacb31") },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("383d5956-1326-4263-b6a6-6d8bf9be4728") }
                });

            // ============================================
            // DATOS INICIALES - RolePermissions
            // ============================================
            migrationBuilder.InsertData(
                table: "RolePermissions",
                schema: "accesscontrol",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("00e2ee71-5816-4ce6-aefd-cda8b1f10d0f") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("030640f8-5b1c-4a44-ac0a-7702a4e63008") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("0c90e6be-52f5-4378-b6d5-a3c5b1b5cdb6") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("0f4de161-4aa0-4e78-bbe5-7d083cddd604") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("16156551-e75f-441a-b279-97c5419fbc1e") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("2145dbdc-3e3b-4074-b3d6-c1c64e4f8124") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("24ef1acc-9a55-4b86-9868-a7fe2d01ca6f") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("2660d239-eafc-490b-8cb0-03e39120a244") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("282da9b9-ba97-4d38-a267-2339daeb3957") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("286989b0-fe3e-42dc-862e-8cfb9a6c47a8") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("2c85aa39-60b0-41e8-9d60-1e12c37918fd") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("355c217f-3ab3-4f82-a753-8a339610a27d") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("4792f730-b1ca-4af2-a281-8a97250ffefb") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("4d1e7d33-3470-4d6d-8d6e-e8227b73a3fd") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("4e2f78f0-6d4c-4e65-9029-efa4dbb519b1") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("4f134eb3-2dc0-4806-8519-41cb7727efc6") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("501cadb6-4f7c-43b8-b2bb-3463ee2d5440") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("5412ed73-19a3-487a-bcfd-beae36c5a27e") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("5449c302-93c8-4670-ba28-0567dba694ba") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("5c2cb257-2d18-4870-9675-da43b772a5cb") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("61731b86-0223-44f5-80be-eb8e9dd8327e") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("6d1c2462-fa3e-4a68-86e8-b6896cdaccd6") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("6e2281c1-e626-4daf-b5ec-7da12deaa6a5") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("73cb2962-aef2-42a4-8407-cf0869625ede") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("76aeb57f-cdf2-40db-b02e-cc175e7253c0") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("80c6834a-7ddf-4232-987d-638196cb3972") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("81addf88-c92f-4d4a-90e2-1969e80a4551") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("8ac8e2be-83ae-4bbf-a599-91dab83cd1f7") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("8e395540-fde7-4f2c-a360-62a9acb08c39") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("94737ae8-877a-4d01-99ea-8b9f69154f56") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("962f2a3c-5891-4836-b556-dbe287d17876") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("a01ec000-518f-4fec-a8b5-30af492eedc9") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("a04e82f0-1cb1-4fd3-a3a3-8d945ab90724") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("a15c1b24-b3ac-44d4-87c7-b645b54bd32c") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("a6e82580-5cef-48aa-acad-edcaa825ade6") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("ab8b6e77-fc91-49a2-9800-a859a1e6c648") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("af015994-520b-4e03-abb0-cf0cf9975675") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("b2a3f726-dcf7-4073-ba61-ecd5dd09dd92") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("b3a75e58-e3c2-4519-9cae-ac0653ac0eb3") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("b47fac30-82cb-46da-85b8-b1046ae6916d") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("bba7db9f-4562-4849-b620-7a47f100f4cb") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("bc6c8488-06c8-4246-8253-a80ac8b5625d") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("c236764a-03a2-428a-b1e2-3819dbd2bfa6") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("c5f88de6-304a-44c2-a28d-178d33d49152") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("c8354179-dc42-45b7-a1b8-34b7b1073f87") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("c9fe2949-b248-4c13-9e3e-aba40a21b1d9") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("cc0d0fde-68d4-44c0-8c00-e7ee24501413") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("d175300f-22c8-487e-95eb-54f32fc1b5b5") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("d8e9507a-5b12-4d2a-8c98-1a52b1466a93") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("dc82f99d-6f48-4fe7-b3f4-49260b8ac9d8") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("e1d93604-6a4b-4468-ae77-00ea465042a0") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("f0b694ec-335c-4f3d-b4f3-58e33e423737") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("f12b05f9-c256-4dcd-bcfa-1b791e4cdc82") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("f5054155-11c5-4077-ac9a-4d02733e4ffe") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("f7c3c0a4-2b6d-4e92-9a0d-6e1d948a43f8") },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("cc0d0fde-68d4-44c0-8c00-e7ee24501413") },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("c236764a-03a2-428a-b1e2-3819dbd2bfa6") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar datos de Users
            migrationBuilder.DeleteData(
                table: "Users",
                schema: "accesscontrol",
                keyColumn: "Id",
                keyValue: new Guid("57a98f97-9271-4aff-83d4-09332cfacb31"));
            migrationBuilder.DeleteData(
                table: "Users",
                schema: "accesscontrol",
                keyColumn: "Id",
                keyValue: new Guid("383d5956-1326-4263-b6a6-6d8bf9be4728"));

            // Eliminar datos de Roles
            migrationBuilder.DeleteData(
                table: "Roles",
                schema: "accesscontrol",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
            migrationBuilder.DeleteData(
                table: "Roles",
                schema: "accesscontrol",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));

            // Eliminar datos de Permissions
            var permissionIds = new Guid[]
            {
                new Guid("00e2ee71-5816-4ce6-aefd-cda8b1f10d0f"),
                new Guid("030640f8-5b1c-4a44-ac0a-7702a4e63008"),
                new Guid("0c90e6be-52f5-4378-b6d5-a3c5b1b5cdb6"),
                new Guid("0f4de161-4aa0-4e78-bbe5-7d083cddd604"),
                new Guid("16156551-e75f-441a-b279-97c5419fbc1e"),
                new Guid("2145dbdc-3e3b-4074-b3d6-c1c64e4f8124"),
                new Guid("24ef1acc-9a55-4b86-9868-a7fe2d01ca6f"),
                new Guid("2660d239-eafc-490b-8cb0-03e39120a244"),
                new Guid("282da9b9-ba97-4d38-a267-2339daeb3957"),
                new Guid("286989b0-fe3e-42dc-862e-8cfb9a6c47a8"),
                new Guid("2c85aa39-60b0-41e8-9d60-1e12c37918fd"),
                new Guid("355c217f-3ab3-4f82-a753-8a339610a27d"),
                new Guid("4792f730-b1ca-4af2-a281-8a97250ffefb"),
                new Guid("4d1e7d33-3470-4d6d-8d6e-e8227b73a3fd"),
                new Guid("4e2f78f0-6d4c-4e65-9029-efa4dbb519b1"),
                new Guid("4f134eb3-2dc0-4806-8519-41cb7727efc6"),
                new Guid("501cadb6-4f7c-43b8-b2bb-3463ee2d5440"),
                new Guid("5412ed73-19a3-487a-bcfd-beae36c5a27e"),
                new Guid("5449c302-93c8-4670-ba28-0567dba694ba"),
                new Guid("5c2cb257-2d18-4870-9675-da43b772a5cb"),
                new Guid("61731b86-0223-44f5-80be-eb8e9dd8327e"),
                new Guid("6d1c2462-fa3e-4a68-86e8-b6896cdaccd6"),
                new Guid("6e2281c1-e626-4daf-b5ec-7da12deaa6a5"),
                new Guid("73cb2962-aef2-42a4-8407-cf0869625ede"),
                new Guid("76aeb57f-cdf2-40db-b02e-cc175e7253c0"),
                new Guid("80c6834a-7ddf-4232-987d-638196cb3972"),
                new Guid("81addf88-c92f-4d4a-90e2-1969e80a4551"),
                new Guid("8ac8e2be-83ae-4bbf-a599-91dab83cd1f7"),
                new Guid("8e395540-fde7-4f2c-a360-62a9acb08c39"),
                new Guid("94737ae8-877a-4d01-99ea-8b9f69154f56"),
                new Guid("962f2a3c-5891-4836-b556-dbe287d17876"),
                new Guid("a01ec000-518f-4fec-a8b5-30af492eedc9"),
                new Guid("a04e82f0-1cb1-4fd3-a3a3-8d945ab90724"),
                new Guid("a15c1b24-b3ac-44d4-87c7-b645b54bd32c"),
                new Guid("a6e82580-5cef-48aa-acad-edcaa825ade6"),
                new Guid("ab8b6e77-fc91-49a2-9800-a859a1e6c648"),
                new Guid("af015994-520b-4e03-abb0-cf0cf9975675"),
                new Guid("b2a3f726-dcf7-4073-ba61-ecd5dd09dd92"),
                new Guid("b3a75e58-e3c2-4519-9cae-ac0653ac0eb3"),
                new Guid("b47fac30-82cb-46da-85b8-b1046ae6916d"),
                new Guid("bba7db9f-4562-4849-b620-7a47f100f4cb"),
                new Guid("bc6c8488-06c8-4246-8253-a80ac8b5625d"),
                new Guid("c236764a-03a2-428a-b1e2-3819dbd2bfa6"),
                new Guid("c5f88de6-304a-44c2-a28d-178d33d49152"),
                new Guid("c8354179-dc42-45b7-a1b8-34b7b1073f87"),
                new Guid("c9fe2949-b248-4c13-9e3e-aba40a21b1d9"),
                new Guid("cc0d0fde-68d4-44c0-8c00-e7ee24501413"),
                new Guid("d175300f-22c8-487e-95eb-54f32fc1b5b5"),
                new Guid("d8e9507a-5b12-4d2a-8c98-1a52b1466a93"),
                new Guid("dc82f99d-6f48-4fe7-b3f4-49260b8ac9d8"),
                new Guid("e1d93604-6a4b-4468-ae77-00ea465042a0"),
                new Guid("f0b694ec-335c-4f3d-b4f3-58e33e423737"),
                new Guid("f12b05f9-c256-4dcd-bcfa-1b791e4cdc82"),
                new Guid("f5054155-11c5-4077-ac9a-4d02733e4ffe"),
                new Guid("f7c3c0a4-2b6d-4e92-9a0d-6e1d948a43f8")
            };
            foreach (var permId in permissionIds)
            {
                migrationBuilder.DeleteData(
                    table: "Permissions",
                    schema: "accesscontrol",
                    keyColumn: "Id",
                    keyValue: permId);
            }

            // Eliminar datos de UserTypes
            migrationBuilder.DeleteData(
                table: "UserTypes",
                schema: "accesscontrol",
                keyColumn: "Id",
                keyValue: new Guid("82d97e80-eba4-45ea-9f41-0d85e0a7413d"));
            migrationBuilder.DeleteData(
                table: "UserTypes",
                schema: "accesscontrol",
                keyColumn: "Id",
                keyValue: new Guid("d13d0d11-acc7-4c8f-8cb9-e6b2706cc89a"));

            // Eliminar datos de menu_items
            for (int i = 1; i <= 25; i++)
            {
                migrationBuilder.DeleteData(
                    table: "menu_items",
                    schema: "public",
                    keyColumn: "id",
                    keyValue: i);
            }
        }
    }
}
