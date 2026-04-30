using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Domain.Entities.AccessControl;

namespace RestaurantManager.Infrastructure.DbContexts;

public partial class RestaurantManagerDbContext : DbContext
{
    static RestaurantManagerDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public RestaurantManagerDbContext(DbContextOptions<RestaurantManagerDbContext> options)
        : base(options)
    {
    }

    // Access Control
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<RolePermission> RolePermissions { get; set; }
    public virtual DbSet<Session> Sessions { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<UserType> UserTypes { get; set; }

    // Restaurant Business
    public virtual DbSet<Dish> Dishes { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var dateTimeAsUnspecifiedConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Unspecified ? v : DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
            v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified));

        var nullableDateTimeAsUnspecifiedConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue
                ? (v.Value.Kind == DateTimeKind.Unspecified ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified))
                : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified) : v);

        // Declarar enums de PostgreSQL en el esquema public
        modelBuilder.HasPostgresEnum<DishCategory>("public", "categoria_menu");
        modelBuilder.HasPostgresEnum<OrderStatus>("public", "order_status");

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Permissions_pkey");

            entity.ToTable("Permissions", "accesscontrol");

            entity.HasIndex(e => e.Name, "UQ_Permissions_Name").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("Id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("Description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("Name");
            entity.Property(e => e.Status)
                .HasColumnName("Status");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()")
                .HasColumnName("CreatedAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("UpdatedAt");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Roles_pkey");

            entity.ToTable("Roles", "accesscontrol");

            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnName("Id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("Name");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("Description");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("CreatedAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("UpdatedAt");

            // Relación configurada através de la entidad RolePermission explícita
            // No necesitamos configuración muchos-a-muchos automática
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Sessions_pkey");

            entity.ToTable("Sessions", "accesscontrol");

            entity.Property(e => e.Id)
                .HasColumnName("Id");
            entity.Property(e => e.Expires)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Expires");
            entity.Property(e => e.SessionToken)
                .HasColumnName("SessionToken");
            entity.Property(e => e.UserId).HasColumnName("UserId");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Sessions_UserId_fkey");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserTypes_pkey");

            entity.ToTable("UserTypes", "accesscontrol");

            entity.HasIndex(e => e.Name, "unique name").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("Id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("Name");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("Description");
            entity.Property(e => e.Status)
                .HasColumnName("Status");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("CreatedAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("UpdatedAt");
            entity.Property(e => e.Theme)
                .HasColumnType("character varying")
                .HasColumnName("Theme");
            entity.Property(e => e.DefaultLandingPage)
                .HasColumnType("character varying")
                .HasColumnName("DefaultLandingPage");
            entity.Property(e => e.LogoUrl)
                .HasColumnType("character varying")
                .HasColumnName("LogoUrl");
            entity.Property(e => e.Language)
                .HasColumnType("character varying")
                .HasColumnName("Language");
            entity.Property(e => e.AdditionalConfig)
                .HasColumnType("character varying")
                .HasColumnName("AdditionalConfig");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.ToTable("Users", "accesscontrol");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("Id");
            entity.Property(e => e.Name)
                .HasColumnName("Name");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("Address");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("CreatedAt");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("Email");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .HasColumnName("Password");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("Image");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("Phone");
            entity.Property(e => e.UserTypeId)
                .HasColumnName("UserTypeId");
            entity.Property(e => e.ExtraData)
                .HasColumnType("text")
                .HasColumnName("ExtraData")
                .HasDefaultValueSql("'{}'::text");
            entity.Property(e => e.Status)
                .HasColumnName("Status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("UpdatedAt");

            entity.HasOne(d => d.UserType).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserTypeId)
                .HasConstraintName("Users_UserTypeId_fkey");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<UserRole>(
                    l => l.HasOne<Role>(ur => ur.Role).WithMany()
                        .HasForeignKey(ur => ur.RoleId)
                        .HasConstraintName("UserRoles_RoleId_fkey"),
                    r => r.HasOne<User>(ur => ur.User).WithMany()
                        .HasForeignKey(ur => ur.UserId)
                        .HasConstraintName("UserRoles_UserId_fkey"),
                    j =>
                    {
                        j.HasKey(ur => new { ur.UserId, ur.RoleId }).HasName("UserRoles_pkey");
                        j.ToTable("UserRoles", "accesscontrol");
                    });
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId }).HasName("RolePermissions_pkey");

            entity.ToTable("RolePermissions", "accesscontrol");

            entity.Property(e => e.RoleId)
                .HasColumnName("RoleId");
            entity.Property(e => e.PermissionId)
                .HasColumnName("PermissionId");

            entity.HasOne(d => d.Role).WithMany(r => r.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("RolePermissions_RoleId_fkey");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("RolePermissions_PermissionId_fkey");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Menus__3213E83F");

            entity.ToTable("Menus", "accesscontrol");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("Id");
            entity.Property(e => e.Label)
                .IsRequired()
                .HasColumnName("Label");
            entity.Property(e => e.Icon)
                .IsRequired()
                .HasColumnName("Icon");
            entity.Property(e => e.Route)
                .IsRequired()
                .HasColumnName("Route");
            entity.Property(e => e.Order)
                .HasColumnName("Order");
            entity.Property(e => e.IsGroup)
                .HasColumnName("IsGroup");
            entity.Property(e => e.ParentId)
                .HasColumnName("ParentId");
            entity.Property(e => e.Status)
                .HasColumnName("Status");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("CreatedAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("UpdatedAt");
        });

        // ===== RESTAURANT BUSINESS ENTITIES =====

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("menu_items_pkey");

            entity.ToTable("menu_items", "public");

            entity.HasIndex(e => e.Name, "menu_items_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Description)
                .HasColumnName("description");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Category)
                .HasColumnName("category")
                .HasColumnType("categoria_menu");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("is_available");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.HasIndex(e => e.OrderDate, "idx_orders_date");
            entity.HasIndex(e => e.Status, "idx_orders_status");

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.TableNumber)
                .HasColumnName("table_number");
            entity.Property(e => e.Waiter)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("waiter_name");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("order_status");

            entity.HasMany(e => e.OrderItems)
                .WithOne(e => e.Order)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_details_pkey");

            entity.ToTable("order_details");

            entity.HasIndex(e => e.DishId, "idx_details_item_id");
            entity.HasIndex(e => e.OrderId, "idx_details_order_id");

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.OrderId)
                .HasColumnName("order_id");
            entity.Property(e => e.DishId)
                .HasColumnName("menu_item_id");
            entity.Property(e => e.Quantity)
                .HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");
            entity.Property(e => e.Notes)
                .HasColumnName("notes")
                .HasDefaultValueSql("''::text");

            entity.HasOne(e => e.Dish)
                .WithMany(e => e.OrderItems)
                .HasForeignKey(e => e.DishId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_details_menu_item");

            entity.HasOne(e => e.Order)
                .WithMany(e => e.OrderItems)
                .HasForeignKey(e => e.OrderId)
                .HasConstraintName("fk_order_details_order");
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(DateTime) && property.ClrType != typeof(DateTime?))
                {
                    continue;
                }

                if (!string.Equals(property.GetColumnType(), "timestamp without time zone", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                property.SetValueConverter(property.ClrType == typeof(DateTime)
                    ? dateTimeAsUnspecifiedConverter
                    : nullableDateTimeAsUnspecifiedConverter);
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}
