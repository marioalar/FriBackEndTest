using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FRITest.Models;

public partial class FritestContext : DbContext
{
    public FritestContext()
    {
    }

    public FritestContext(DbContextOptions<FritestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Meal> Meals { get; set; }
  //  public virtual DbSet<MealsL> MealsL { get; set; }
    public virtual DbSet<MealsUser> MealsUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Meal>(entity =>
        {
            entity.HasKey(e => e.idMeal).HasName("PK__meals__C26D6F241624C5AA");

            entity.ToTable("meals");

            entity.Property(e => e.idMeal)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("idMeal");
            entity.Property(e => e.strCategory)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("strCategory");
            entity.Property(e => e.strMeal)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("strMeal");
            entity.Property(e => e.strMealThumb)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("strMealThumb");
            entity.Property(e => e.strYoutube)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("strYoutube");
        });

        modelBuilder.Entity<MealsUser>(entity =>
        {
            entity.HasKey(e => new { e.IdMeal, e.IdUser }).HasName("FK_User_Meals");

            entity.ToTable("MealsUser");

            entity.Property(e => e.IdMeal)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("idMeal");
            entity.Property(e => e.IdUser).HasColumnName("idUser");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__User__3717C9828A73994C");

            entity.ToTable("User");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("idUser");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
