using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProductSellPurchase.DBContext;

public partial class ProductSellPurchaseDBContext : DbContext
{
    public ProductSellPurchaseDBContext(DbContextOptions<ProductSellPurchaseDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<PageName> PageNames { get; set; }

    public virtual DbSet<PermissionType> PermissionTypes { get; set; }

    public virtual DbSet<ProductList> ProductLists { get; set; }

    public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; }

    public virtual DbSet<SellDetail> SellDetails { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_StateId_State_StateId");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country");
        });

        modelBuilder.Entity<PageName>(entity =>
        {
            entity.HasKey(e => e.PageId).HasName("PK_PageName");

            entity.Property(e => e.PageName1).HasColumnName("PageName");
        });

        modelBuilder.Entity<PermissionType>(entity =>
        {
            entity.HasKey(e => e.PermissionTypeId).HasName("PK_PermissionTypeList");
        });

        modelBuilder.Entity<ProductList>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_Product");

            entity.ToTable("ProductList");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductListCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("fk_Product_CreatedBy_Users_UserId");

            entity.HasOne(d => d.ModifyByNavigation).WithMany(p => p.ProductListModifyByNavigations)
                .HasForeignKey(d => d.ModifyBy)
                .HasConstraintName("fk_Product_ModifyBy_Users_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.ProductListUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Product_UserId_Users_UserId");
        });

        modelBuilder.Entity<PurchaseDetail>(entity =>
        {
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PurchaseDate).HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseDetails_ProductId_ProductList_ProductId");

            entity.HasOne(d => d.User).WithMany(p => p.PurchaseDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseDetails_UserId_Users_UserId");
        });

        modelBuilder.Entity<SellDetail>(entity =>
        {
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SellDate).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.SellDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SellDetails_ProductId_ProductList_ProductId");

            entity.HasOne(d => d.User).WithMany(p => p.SellDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SellDetails_UserId_Users_UserId");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("State");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_State_CountryId_Country_CountryId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ResetTokenExpiry).HasColumnType("datetime");

            entity.HasOne(d => d.UserType).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_UserTypeId_UserTypes_UserTypeId");
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.ToTable("UserPermission");

            entity.HasOne(d => d.Page).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermission_PageId_PageNames_PageId");

            entity.HasOne(d => d.PermissionType).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.PermissionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermission_PermissionTypeId_PermissionTypes_PermissionTypeId");

            entity.HasOne(d => d.UserType).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPermission_UserTypeId_UserTypes_UserTypeId");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.UserTypeId).HasName("PK_UserType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
