using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models;

public partial class LibraryContext : DbContext
{
    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookArrival> BookArrivals { get; set; }

    public virtual DbSet<Borrow> Borrows { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<LikedBook> LikedBooks { get; set; }

    public virtual DbSet<OperationHistory> OperationHistories { get; set; }

    public virtual DbSet<Shelter> Shelters { get; set; }

    public virtual DbSet<User> Users { get; set; }

   /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DbConnection2");*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Book_PK");

            entity.ToTable("Book");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Author)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CategoryId).HasColumnName("Category_ID");
            entity.Property(e => e.Cover)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Publisher)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ShelterId).HasColumnName("Shelter_ID");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Book_Category_FK");

            entity.HasOne(d => d.Shelter).WithMany(p => p.Books)
                .HasForeignKey(d => d.ShelterId)
                .HasConstraintName("Book_Shelter_FK");
        });

        modelBuilder.Entity<BookArrival>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BookArrival_PK");

            entity.ToTable("BookArrival", tb => tb.HasTrigger("T_AfterInsert_BookArrival"));

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookId).HasColumnName("Book_ID");
            entity.Property(e => e.DateTime).HasColumnType("datetime");
            entity.Property(e => e.ShelterId).HasColumnName("Shelter_ID");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Book).WithMany(p => p.BookArrivals)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BookArrival_Book_FK");

            entity.HasOne(d => d.Shelter).WithMany(p => p.BookArrivals)
                .HasForeignKey(d => d.ShelterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BookArrival_Shelter_FK");

            entity.HasOne(d => d.User).WithMany(p => p.BookArrivals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BookArrival_User_FK");
        });

        modelBuilder.Entity<Borrow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Borrow_PK");

            entity.ToTable("Borrow", tb =>
                {
                    tb.HasTrigger("T_AfterInsert_Borrow");
                    tb.HasTrigger("T_AfterUpdate_ReturnShelterID");
                });

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BeginDate).HasColumnType("datetime");
            entity.Property(e => e.BookId).HasColumnName("Book_ID");
            entity.Property(e => e.BorrowShelterId).HasColumnName("Borrow_Shelter_ID");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.ReturnShelterId).HasColumnName("Return_Shelter_ID");
            entity.Property(e => e.ReturnTime).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Book).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Borrow_Book_FK");

            entity.HasOne(d => d.BorrowShelter).WithMany(p => p.BorrowBorrowShelters)
                .HasForeignKey(d => d.BorrowShelterId)
                .HasConstraintName("Borrow_Shelter_FK");

            entity.HasOne(d => d.ReturnShelter).WithMany(p => p.BorrowReturnShelters)
                .HasForeignKey(d => d.ReturnShelterId)
                .HasConstraintName("Return_Shelter_FK");

            entity.HasOne(d => d.User).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Borrow_User_FK");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Category_PK");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LikedBook>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LikedBook_PK");

            entity.ToTable("LikedBook");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookId).HasColumnName("Book_ID");
            entity.Property(e => e.LikedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Book).WithMany(p => p.LikedBooks)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("LikedBook_Book_FK");

            entity.HasOne(d => d.User).WithMany(p => p.LikedBooks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("LikedBook_User_FK");
        });

        modelBuilder.Entity<OperationHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OperationHistory_PK");

            entity.ToTable("OperationHistory");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateTime).HasColumnType("datetime");
            entity.Property(e => e.OperationDescription).IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.OperationHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OperationHistory_User_FK");
        });

        modelBuilder.Entity<Shelter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Shelter_PK");

            entity.ToTable("Shelter");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_PK");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
