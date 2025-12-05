using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Projectbakamitai.Models;

namespace Projectbakamitai.Data;

public partial class ProjectBakamitaiContext : DbContext
{
    public ProjectBakamitaiContext()
    {
    }

    public ProjectBakamitaiContext(DbContextOptions<ProjectBakamitaiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<PlayerProfile> PlayerProfiles { get; set; }

    public virtual DbSet<ShopProduct> ShopProducts { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-28HSMTC\\SQLEXPRESS;Initial Catalog=Project_Bakamitai;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InvenId).HasName("PK__Inventor__596690ED61B24578");

            entity.ToTable("Inventory");

            entity.Property(e => e.InvenId).HasColumnName("InvenID");
            entity.Property(e => e.PurchasedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Item).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__ItemI__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__UserI__5FB337D6");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Items__727E838B00568601");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsShow).HasDefaultValue(true);
            entity.Property(e => e.ItemName).HasMaxLength(100);
            entity.Property(e => e.ItemType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PriceGem).HasDefaultValue(0);
            entity.Property(e => e.PriceGold).HasDefaultValue(0);
            entity.Property(e => e.ProductImage)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PlayerProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__PlayerPr__290C88E4DFC37221");

            entity.ToTable("PlayerProfile");

            entity.HasIndex(e => e.UserId, "UQ__PlayerPr__1788CC4D1964DE51").IsUnique();

            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.DisplayName).HasMaxLength(50);
            entity.Property(e => e.Exps).HasDefaultValue(0);
            entity.Property(e => e.Gem).HasDefaultValue(0);
            entity.Property(e => e.Gold).HasDefaultValue(0);
            entity.Property(e => e.Levels).HasDefaultValue(1);

            entity.HasOne(d => d.User).WithOne(p => p.PlayerProfile)
                .HasForeignKey<PlayerProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlayerProfi__Gem__52593CB8");
        });

        modelBuilder.Entity<ShopProduct>(entity =>
        {
            entity.HasKey(e => e.ShopItemId).HasName("PK__ShopProd__C4AC2954C2B71598");

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

            entity.HasOne(d => d.Item).WithMany(p => p.ShopProducts)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShopProdu__ItemI__5AEE82B9");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6BFE9D7143");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrencyType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.TransactionType)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Item).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__ItemI__66603565");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__UserI__656C112C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CF8E434D7");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105347A119396").IsUnique();

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(32);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
