using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjectBakamitai.Models;

namespace ProjectBakamitai.Data;

public partial class ProjectbakamitaiContext : DbContext
{
    public ProjectbakamitaiContext()
    {
    }

    public ProjectbakamitaiContext(DbContextOptions<ProjectbakamitaiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Character> Characters { get; set; }

    public virtual DbSet<CharacterMission> CharacterMissions { get; set; }

    public virtual DbSet<GameMode> GameModes { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Mission> Missions { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-28HSMTC\\SQLEXPRESS;Initial Catalog=projectbakamitai;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.CharacterId).HasName("PK__Characte__757BCA4025F2E3A2");

            entity.Property(e => e.CharacterId)
                .ValueGeneratedOnAdd()
                .HasColumnName("CharacterID");
            entity.Property(e => e.CharacterName).HasMaxLength(40);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Experience).HasDefaultValue(0);
            entity.Property(e => e.GamemodeId).HasColumnName("GamemodeID");
            entity.Property(e => e.Gold).HasDefaultValue(0);
            entity.Property(e => e.Heatlh).HasDefaultValue(100);
            entity.Property(e => e.Hunger).HasDefaultValue(100);
            entity.Property(e => e.PlayerId).HasColumnName("PlayerID");

            entity.HasOne(d => d.Gamemode).WithMany(p => p.Characters)
                .HasForeignKey(d => d.GamemodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Character__Gamem__5441852A");

            entity.HasOne(d => d.Player).WithMany(p => p.Characters)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Character__Playe__534D60F1");
        });

        modelBuilder.Entity<CharacterMission>(entity =>
        {
            entity.HasKey(e => new { e.CharacterId, e.MissionId }).HasName("PK__Characte__231631C50A0B0C61");

            entity.ToTable("CharacterMission");

            entity.Property(e => e.CharacterId).HasColumnName("CharacterID");
            entity.Property(e => e.MissionId).HasColumnName("MissionID");
            entity.Property(e => e.CompletionDate).HasColumnType("datetime");
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);

            entity.HasOne(d => d.Character).WithMany(p => p.CharacterMissions)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Character__Chara__5BE2A6F2");

            entity.HasOne(d => d.Mission).WithMany(p => p.CharacterMissions)
                .HasForeignKey(d => d.MissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Character__Missi__5CD6CB2B");
        });

        modelBuilder.Entity<GameMode>(entity =>
        {
            entity.HasKey(e => e.GamemodeId).HasName("PK__GameMode__AD478C328B84FBB1");

            entity.ToTable("GameMode");

            entity.Property(e => e.GamemodeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("GamemodeID");
            entity.Property(e => e.ModeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.CharacterId, e.ItemId }).HasName("PK__Inventor__D25C227E7C5585A5");

            entity.ToTable("Inventory");

            entity.Property(e => e.CharacterId).HasColumnName("CharacterID");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.AcquireDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Character).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.CharacterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Chara__693CA210");

            entity.HasOne(d => d.Item).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__ItemI__6A30C649");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Items__727E83EBDE17199D");

            entity.Property(e => e.ItemId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ItemID");
            entity.Property(e => e.ItemName).HasMaxLength(50);
            entity.Property(e => e.ItemType).HasMaxLength(50);
        });

        modelBuilder.Entity<Mission>(entity =>
        {
            entity.HasKey(e => e.MissionId).HasName("PK__Missions__66DFB854DC0F043D");

            entity.Property(e => e.MissionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("MissionID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Expreward)
                .HasDefaultValue(0)
                .HasColumnName("EXPReward");
            entity.Property(e => e.GoldReward).HasDefaultValue(0);
            entity.Property(e => e.Title).HasMaxLength(80);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK__Players__4A4E74A89C2D905B");

            entity.Property(e => e.PlayerId)
                .ValueGeneratedOnAdd()
                .HasColumnName("PlayerID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(1);
            entity.Property(e => e.PlayerName).HasMaxLength(50);
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.ShopId).HasName("PK__Shop__67C55629DDC1BF99");

            entity.ToTable("Shop");

            entity.Property(e => e.ShopId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ShopID");
            entity.Property(e => e.ShopName).HasMaxLength(40);

            entity.HasMany(d => d.Items).WithMany(p => p.Shops)
                .UsingEntity<Dictionary<string, object>>(
                    "ShopItem",
                    r => r.HasOne<Item>().WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ShopItem__ItemID__6477ECF3"),
                    l => l.HasOne<Shop>().WithMany()
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ShopItem__ShopID__6383C8BA"),
                    j =>
                    {
                        j.HasKey("ShopId", "ItemId").HasName("PK__ShopItem__C0E2BE17DB112ECB");
                        j.ToTable("ShopItem");
                        j.IndexerProperty<byte>("ShopId").HasColumnName("ShopID");
                        j.IndexerProperty<byte>("ItemId").HasColumnName("ItemID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
