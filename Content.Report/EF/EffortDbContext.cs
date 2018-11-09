using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Content.Report.EF
{
    public partial class EffortDbContext : DbContext
    {
        public EffortDbContext()
        {
        }

        public EffortDbContext(DbContextOptions<EffortDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Effortstatisticqa> Effortstatisticqa { get; set; }
        public virtual DbSet<Effortstatisticteam> Effortstatisticteam { get; set; }
        public virtual DbSet<Effortstatisticuser> Effortstatisticuser { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<Teamprojects> Teamprojects { get; set; }

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
            modelBuilder.Entity<Effortstatisticqa>(entity =>
            {
                entity.ToTable("effortstatisticqa", "taggie.content.api.dev");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.EffortCount).HasColumnType("int(11)");

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

            modelBuilder.Entity<Effortstatisticteam>(entity =>
            {
                entity.ToTable("effortstatisticteam", "taggie.content.api.dev");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.FinishedCount).HasColumnType("int(11)");

                entity.Property(e => e.IncorrectCount).HasColumnType("int(11)");

                entity.Property(e => e.TeamId).HasColumnType("int(11)");

                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Effortstatisticuser>(entity =>
            {
                entity.ToTable("effortstatisticuser", "taggie.content.api.dev");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.FinishedCount).HasColumnType("int(11)");

                entity.Property(e => e.IncorrectCount).HasColumnType("int(11)");

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
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("team", "taggie.content.api.dev");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnType("tinyint(4)");

                entity.Property(e => e.TeamName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UsersCount).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Teamprojects>(entity =>
            {
                entity.ToTable("teamprojects", "taggie.content.api.dev");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("FK_jProjectId");

                entity.HasIndex(e => e.TeamId)
                    .HasName("FK_jTeamId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.ProjectId).HasColumnType("int(11)");

                entity.Property(e => e.TeamId).HasColumnType("int(11)");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Teamprojects)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_jProjectId");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Teamprojects)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_jTeamId");
            });
        }
    }
}
