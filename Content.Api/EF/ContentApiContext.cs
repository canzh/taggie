using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace Content.Api.EF
{
    public partial class ContentApiContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public ContentApiContext()
        {
        }

        public ContentApiContext(ILoggerFactory loggerFactory, DbContextOptions<ContentApiContext> options)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Contentfile> Contentfile { get; set; }
        public virtual DbSet<Contentfilepath> Contentfilepath { get; set; }
        public virtual DbSet<Filecategory> Filecategory { get; set; }
        public virtual DbSet<FileSubcategory> Filesubcategory { get; set; }
        public virtual DbSet<Subcategory> Subcategory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=root;database=taggie.content.api.dev");
            }

            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category", "taggie.content.api.dev");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnType("tinyint(1)");
            });

            modelBuilder.Entity<Contentfile>(entity =>
            {
                entity.ToTable("contentfile", "taggie.content.api.dev");

                entity.HasIndex(e => e.FilePathId)
                    .HasName("FK_Source_PathId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FilePathId).HasColumnType("int(255)");

                entity.Property(e => e.FinishedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnType("tinyint(1)");

                entity.Property(e => e.VerifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.FilePath)
                    .WithMany(p => p.Contentfile)
                    .HasForeignKey(d => d.FilePathId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Source_PathId");
            });

            modelBuilder.Entity<Contentfilepath>(entity =>
            {
                entity.ToTable("contentfilepath", "taggie.content.api.dev");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FullPath)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Filecategory>(entity =>
            {
                entity.ToTable("filecategory", "taggie.content.api.dev");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("FK_CategoryId");

                entity.HasIndex(e => e.FileId)
                    .HasName("FK_FileId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.FileId).HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Filecategory)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CategoryId");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.Filecategory)
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_FileId");
            });

            modelBuilder.Entity<FileSubcategory>(entity =>
            {
                entity.ToTable("filesubcategory", "taggie.content.api.dev");

                entity.HasIndex(e => e.FileId)
                    .HasName("FK_FileId_Sub");

                entity.HasIndex(e => e.SubCategoryId)
                    .HasName("FK_SubCategoryId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FileId).HasColumnType("int(11)");

                entity.Property(e => e.SubCategoryId).HasColumnType("int(11)");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.Filesubcategory)
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_FileId_Sub");

                entity.HasOne(d => d.SubCategory)
                    .WithMany(p => p.Filesubcategory)
                    .HasForeignKey(d => d.SubCategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_SubCategoryId");
            });

            modelBuilder.Entity<Subcategory>(entity =>
            {
                entity.ToTable("subcategory", "taggie.content.api.dev");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnType("tinyint(1)");
            });
        }
    }
}
