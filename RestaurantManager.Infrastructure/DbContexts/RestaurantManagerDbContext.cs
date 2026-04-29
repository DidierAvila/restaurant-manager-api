using Microsoft.EntityFrameworkCore;
using RestaurantManager.Domain.Entities.AccessControl;

namespace RestaurantManager.Infrastructure.DbContexts;

public partial class RestaurantManagerDbContext : DbContext
{
    public RestaurantManagerDbContext(DbContextOptions<RestaurantManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<RolePermission> RolePermissions { get; set; }
    public virtual DbSet<Session> Sessions { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissions__3213E83F8986953C");

            entity.ToTable("Permissions", "accesscontrol");

            entity.HasIndex(e => e.Name, "UQ__Permissions__72E12F1B1B3B2B3F").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("Description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Name");
            entity.Property(e => e.Status)
                .HasColumnName("Status");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("CreatedAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("UpdatedAt");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3213E83F8E286D4E");

            entity.ToTable("Roles", "accesscontrol");

            entity.HasIndex(e => e.Name, "UQ__Roles__72E12F1B32068C24").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Name");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("Description");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
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
            entity.HasKey(e => e.Id).HasName("PK__Sessions__3213E83F6D1DFCA1");

            entity.ToTable("Sessions", "accesscontrol");

            entity.HasIndex(e => e.SessionToken, "UQ__Sessions__E598F5C811A2DDCB").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Id");
            entity.Property(e => e.Expires)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Expires");
            entity.Property(e => e.SessionToken)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("SessionToken");
            entity.Property(e => e.UserId).HasColumnName("UserId");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Sessions__UserId__440B1D61");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F49253023");

            entity.ToTable("Users", "accesscontrol");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616447836532").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("CreatedAt");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Email");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("Password");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Image");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Phone");
            entity.Property(e => e.UserTypeId)
                .HasColumnName("UserTypeId");
            entity.Property(e => e.ExtraData)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("ExtraData")
                .HasDefaultValue("{}");
            entity.Property(e => e.Status)
                .HasColumnType("boolean")
                .HasColumnName("Status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("UpdatedAt");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<UserRole>(
                    l => l.HasOne<Role>(ur => ur.Role).WithMany()
                        .HasForeignKey(ur => ur.RoleId)
                        .HasConstraintName("FK__UserRoles__RoleId__4F7CD00D"),
                    r => r.HasOne<User>(ur => ur.User).WithMany()
                        .HasForeignKey(ur => ur.UserId)
                        .HasConstraintName("FK__UserRoles__UserId__4E88ABD4"),
                    j =>
                    {
                        j.HasKey(ur => new { ur.UserId, ur.RoleId }).HasName("PK__UserRoles__6EDEA1531A203E84");
                        j.ToTable("UserRoles", "accesscontrol");
                    });
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId }).HasName("PK__RolePermissions__RoleId_PermissionId");

            entity.ToTable("RolePermissions", "accesscontrol");

            entity.Property(e => e.RoleId)
                .HasColumnName("RoleId");
            entity.Property(e => e.PermissionId)
                .HasColumnName("PermissionId");

            entity.HasOne(d => d.Role).WithMany(r => r.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__RolePermissions__Role");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__RolePermissions__Permission");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Menus__3213E83F");

            entity.ToTable("Menus", "accesscontrol");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
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
                .HasDefaultValueSql("(now())")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("CreatedAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("UpdatedAt");
        });

        base.OnModelCreating(modelBuilder);
    }
}