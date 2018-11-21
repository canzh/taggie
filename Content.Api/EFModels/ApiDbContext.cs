using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Content.Api.EFModels
{
    public partial class ApiDbContext : DbContext
    {
        public ApiDbContext()
        {
        }

        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Contentfile> Contentfile { get; set; }
        public virtual DbSet<Contentfilepath> Contentfilepath { get; set; }
        public virtual DbSet<Effortstatisticqa> Effortstatisticqa { get; set; }
        public virtual DbSet<Effortstatistictag> Effortstatistictag { get; set; }
        public virtual DbSet<Effortstatisticteam> Effortstatisticteam { get; set; }
        public virtual DbSet<Filecategory> Filecategory { get; set; }
        public virtual DbSet<Filesubcategory> Filesubcategory { get; set; }
        public virtual DbSet<Finance> Finance { get; set; }
        public virtual DbSet<Financehistory> Financehistory { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Projectcategory> Projectcategory { get; set; }
        public virtual DbSet<Projectitem> Projectitem { get; set; }
        public virtual DbSet<Projectitemcategories> Projectitemcategories { get; set; }
        public virtual DbSet<Projectitemkeywords> Projectitemkeywords { get; set; }
        public virtual DbSet<Projectitemsubcategories> Projectitemsubcategories { get; set; }
        public virtual DbSet<Projectkeyword> Projectkeyword { get; set; }
        public virtual DbSet<Projectsubcategory> Projectsubcategory { get; set; }
        public virtual DbSet<Subcategory> Subcategory { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<Teamprojects> Teamprojects { get; set; }
        public virtual DbSet<Projectitemeffort> Projectitemeffort { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=root;database=taggie.content.api.dev;Charset=utf8");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Projectitemeffort>(entity =>
            {
                entity.ToTable("projectitemeffort", "taggie.content.api.dev");

                entity.HasIndex(e => e.ProjectItemId)
                    .HasName("FK_ProjectItemId_Effort");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.EffortUserId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EffortUserRole).HasColumnType("tinyint(4)");

                entity.Property(e => e.ProjectItemId).HasColumnType("int(11)");

                entity.HasOne(d => d.ProjectItem)
                    .WithMany(p => p.Projectitemeffort)
                    .HasForeignKey(d => d.ProjectItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectItemId_Effort");
            });

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

                entity.Property(e => e.ParentId).HasColumnType("int(255)");
            });

            modelBuilder.Entity<Effortstatisticqa>(entity =>
            {
                entity.ToTable("effortstatisticqa", "taggie.content.api.dev");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.EffortCount).HasColumnType("int(11)");

                entity.Property(e => e.ProjectId).HasColumnType("int(11)");

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.QauserId)
                    .IsRequired()
                    .HasColumnName("QAUserId")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.QauserName)
                    .IsRequired()
                    .HasColumnName("QAUserName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TeamId).HasColumnType("int(11)");

                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Effortstatistictag>(entity =>
            {
                entity.ToTable("effortstatistictag", "taggie.content.api.dev");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FinishedCount).HasColumnType("int(11)");

                entity.Property(e => e.IncorrectCount).HasColumnType("int(11)");

                entity.Property(e => e.ProjectId).HasColumnType("int(11)");

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TeamId).HasColumnType("int(11)");

                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Effortstatisticteam>(entity =>
            {
                entity.ToTable("effortstatisticteam", "taggie.content.api.dev");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("FK_E_Project");

                entity.HasIndex(e => e.TeamId)
                    .HasName("FK_E_Team");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FinishedCount).HasColumnType("int(11)");

                entity.Property(e => e.IncorrectCount).HasColumnType("int(11)");

                entity.Property(e => e.ProjectId).HasColumnType("int(11)");

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TeamId).HasColumnType("int(11)");

                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasMaxLength(100)
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
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CategoryId");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.Filecategory)
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FileId");
            });

            modelBuilder.Entity<Filesubcategory>(entity =>
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
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FileId_Sub");

                entity.HasOne(d => d.SubCategory)
                    .WithMany(p => p.Filesubcategory)
                    .HasForeignKey(d => d.SubCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubCategoryId");
            });

            modelBuilder.Entity<Finance>(entity =>
            {
                entity.ToTable("finance", "taggie.content.api.dev");

                entity.HasIndex(e => e.TeamId)
                    .HasName("FK_F_Team");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.SettledEffort).HasColumnType("int(11)");

                entity.Property(e => e.TeamId).HasColumnType("int(11)");

                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TotalEffort).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Financehistory>(entity =>
            {
                entity.ToTable("financehistory", "taggie.content.api.dev");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FinanceNumber).HasColumnType("int(11)");

                entity.Property(e => e.FinanceType).HasColumnType("tinyint(4)");

                entity.Property(e => e.TeamId).HasColumnType("int(11)");

                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("project", "taggie.content.api.dev");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BaseDir)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnType("tinyint(4)");

                entity.Property(e => e.TotalItems).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Projectcategory>(entity =>
            {
                entity.ToTable("projectcategory", "taggie.content.api.dev");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("FK_P_Category");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("tinyint(4)");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Projectcategory)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_P_Category");
            });

            modelBuilder.Entity<Projectitem>(entity =>
            {
                entity.ToTable("projectitem", "taggie.content.api.dev");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("FK_Item_Project");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.LocalFileName)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.OriginalUrl)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectId).HasColumnType("int(11)");

                entity.Property(e => e.RelativeDir)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnType("tinyint(4)");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Projectitem)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Item_Project");
            });

            modelBuilder.Entity<Projectitemcategories>(entity =>
            {
                entity.ToTable("projectitemcategories", "taggie.content.api.dev");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("FK_CategoryId");

                entity.HasIndex(e => e.ProjectItemId)
                    .HasName("FK_FileId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AddedByRole).HasColumnType("tinyint(4)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.Property(e => e.ProjectItemId).HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Projectitemcategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PI_Category");

                entity.HasOne(d => d.ProjectItem)
                    .WithMany(p => p.Projectitemcategories)
                    .HasForeignKey(d => d.ProjectItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Category_PI");
            });

            modelBuilder.Entity<Projectitemkeywords>(entity =>
            {
                entity.ToTable("projectitemkeywords", "taggie.content.api.dev");

                entity.HasIndex(e => e.ProjectItemId)
                    .HasName("FK_FileId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AddedByRole).HasColumnType("tinyint(4)");

                entity.Property(e => e.Keywords)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectItemId).HasColumnType("int(11)");

                entity.HasOne(d => d.ProjectItem)
                    .WithMany(p => p.Projectitemkeywords)
                    .HasForeignKey(d => d.ProjectItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Keyword_PI");
            });

            modelBuilder.Entity<Projectitemsubcategories>(entity =>
            {
                entity.ToTable("projectitemsubcategories", "taggie.content.api.dev");

                entity.HasIndex(e => e.ProjectItemId)
                    .HasName("FK_FileId_Sub");

                entity.HasIndex(e => e.SubCategoryId)
                    .HasName("FK_SubCategoryId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AddedByRole).HasColumnType("tinyint(4)");

                entity.Property(e => e.ProjectItemId).HasColumnType("int(11)");

                entity.Property(e => e.SubCategoryId).HasColumnType("int(11)");

                entity.HasOne(d => d.ProjectItem)
                    .WithMany(p => p.Projectitemsubcategories)
                    .HasForeignKey(d => d.ProjectItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PI_Subcategory");

                entity.HasOne(d => d.SubCategory)
                    .WithMany(p => p.Projectitemsubcategories)
                    .HasForeignKey(d => d.SubCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Subcategory_PI");
            });

            modelBuilder.Entity<Projectkeyword>(entity =>
            {
                entity.ToTable("projectkeyword", "taggie.content.api.dev");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("tinyint(4)");
            });

            modelBuilder.Entity<Projectsubcategory>(entity =>
            {
                entity.ToTable("projectsubcategory", "taggie.content.api.dev");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("FK_P_Subcategory");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectId).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("tinyint(4)");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Projectsubcategory)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_P_Subcategory");
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

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("team", "taggie.content.api.dev");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.MemberCount).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("tinyint(4)");

                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Teamprojects>(entity =>
            {
                entity.ToTable("teamprojects", "taggie.content.api.dev");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("FK_jProjectId");

                entity.HasIndex(e => e.TeamId)
                    .HasName("FK_jTeamId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AssignedProjectItems).HasColumnType("int(11)");

                entity.Property(e => e.ProjectId).HasColumnType("int(11)");

                entity.Property(e => e.TeamId).HasColumnType("int(11)");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Teamprojects)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_jProjectId");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Teamprojects)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_jTeamId");
            });
        }
    }
}
