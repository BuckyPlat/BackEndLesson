using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataFirst.Models;

public partial class GameVuiVlContext : DbContext
{
    public GameVuiVlContext()
    {
    }

    public GameVuiVlContext(DbContextOptions<GameVuiVlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-I39HLP5\\SQLEXPRESS;Initial Catalog=GameVuiVL;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__player__3214EC27FF3F48D9");

            entity.ToTable("player");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Username).HasMaxLength(20);
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.ShopId).HasName("PK__shop__E5C424FCCBC6FF55");

            entity.ToTable("shop");

            entity.Property(e => e.ShopId)
                .ValueGeneratedOnAdd()
                .HasColumnName("shopID");
            entity.Property(e => e.ItemName).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
