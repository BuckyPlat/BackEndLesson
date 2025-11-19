using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectBakamitai.Models;

public partial class Gd1Context : DbContext
{
    public Gd1Context()
    {
    }

    public Gd1Context(DbContextOptions<Gd1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Character> Characters { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Inventoryitem> Inventoryitems { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Mission> Missions { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Playermission> Playermissions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-28HSMTC\\SQLEXPRESS;Initial Catalog=GD1;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Characterid).HasName("PK__characte__ADFA1D9738453265");

            entity.ToTable("characters");

            entity.Property(e => e.Characterid)
                .ValueGeneratedOnAdd()
                .HasColumnName("characterid");
            entity.Property(e => e.Charactername)
                .HasMaxLength(40)
                .HasColumnName("charactername");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdat");
            entity.Property(e => e.Experience)
                .HasDefaultValue(0)
                .HasColumnName("experience");
            entity.Property(e => e.Health)
                .HasDefaultValue(100)
                .HasColumnName("health");
            entity.Property(e => e.Hunger)
                .HasDefaultValue(100)
                .HasColumnName("hunger");
            entity.Property(e => e.Level)
                .HasDefaultValue(1)
                .HasColumnName("level");
            entity.Property(e => e.Playerid).HasColumnName("playerid");

            entity.HasOne(d => d.Player).WithMany(p => p.Characters)
                .HasForeignKey(d => d.Playerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_characters_player");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Inventoryid).HasName("PK__inventor__C4B4B87AEAEE2AD0");

            entity.ToTable("inventory");

            entity.Property(e => e.Inventoryid)
                .ValueGeneratedOnAdd()
                .HasColumnName("inventoryid");
            entity.Property(e => e.Characterid).HasColumnName("characterid");

            entity.HasOne(d => d.Character).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.Characterid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_inventory_character");
        });

        modelBuilder.Entity<Inventoryitem>(entity =>
        {
            entity.HasKey(e => e.Inventoryitemid).HasName("PK__inventor__2140CBE356F28FCC");

            entity.ToTable("inventoryitems");

            entity.Property(e => e.Inventoryitemid)
                .ValueGeneratedOnAdd()
                .HasColumnName("inventoryitemid");
            entity.Property(e => e.Inventoryid).HasColumnName("inventoryid");
            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Inventory).WithMany(p => p.Inventoryitems)
                .HasForeignKey(d => d.Inventoryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_inventoryitems_inventory");

            entity.HasOne(d => d.Item).WithMany(p => p.Inventoryitems)
                .HasForeignKey(d => d.Itemid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_inventoryitems_item");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("PK__items__56A22C92EC18C690");

            entity.ToTable("items");

            entity.Property(e => e.Itemid)
                .ValueGeneratedOnAdd()
                .HasColumnName("itemid");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Itemname)
                .HasMaxLength(40)
                .HasColumnName("itemname");
            entity.Property(e => e.Itemtype)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("itemtype");
            entity.Property(e => e.Price).HasColumnName("price");
        });

        modelBuilder.Entity<Mission>(entity =>
        {
            entity.HasKey(e => e.Missionid).HasName("PK__mission__B3CC91934D16EDA0");

            entity.ToTable("mission");

            entity.Property(e => e.Missionid)
                .ValueGeneratedOnAdd()
                .HasColumnName("missionid");
            entity.Property(e => e.Difficulty)
                .HasDefaultValue((byte)1)
                .HasColumnName("difficulty");
            entity.Property(e => e.Missionname)
                .HasMaxLength(50)
                .HasColumnName("missionname");
            entity.Property(e => e.Rewardgold)
                .HasDefaultValue(0)
                .HasColumnName("rewardgold");
            entity.Property(e => e.Rewarditemid).HasColumnName("rewarditemid");

            entity.HasOne(d => d.Rewarditem).WithMany(p => p.Missions)
                .HasForeignKey(d => d.Rewarditemid)
                .HasConstraintName("fk_mission_rewarditem");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Playerid).HasName("PK__player__2CD714799DE48A3C");

            entity.ToTable("player");

            entity.HasIndex(e => e.Email, "UQ__player__AB6E61642C1FF23A").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__player__F3DBC572495255D3").IsUnique();

            entity.Property(e => e.Playerid)
                .ValueGeneratedOnAdd()
                .HasColumnName("playerid");
            entity.Property(e => e.Createdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdate");
            entity.Property(e => e.Email)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(32)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Playermission>(entity =>
        {
            entity.HasKey(e => e.Playermissionid).HasName("PK__playermi__E97B481FB057262C");

            entity.ToTable("playermission");

            entity.Property(e => e.Playermissionid)
                .ValueGeneratedOnAdd()
                .HasColumnName("playermissionid");
            entity.Property(e => e.Characterid).HasColumnName("characterid");
            entity.Property(e => e.Missionid).HasColumnName("missionid");
            entity.Property(e => e.Status)
                .HasDefaultValue((byte)0)
                .HasColumnName("status");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Character).WithMany(p => p.Playermissions)
                .HasForeignKey(d => d.Characterid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_playermission_character");

            entity.HasOne(d => d.Mission).WithMany(p => p.Playermissions)
                .HasForeignKey(d => d.Missionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_playermission_mission");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Transactionid).HasName("PK__transact__9B52C2FADC9AEC1F");

            entity.ToTable("transactions");

            entity.Property(e => e.Transactionid)
                .ValueGeneratedOnAdd()
                .HasColumnName("transactionid");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdat");
            entity.Property(e => e.Description)
                .HasMaxLength(120)
                .HasColumnName("description");
            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Playerid).HasColumnName("playerid");

            entity.HasOne(d => d.Item).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.Itemid)
                .HasConstraintName("fk_transaction_item");

            entity.HasOne(d => d.Player).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.Playerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transaction_player");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
