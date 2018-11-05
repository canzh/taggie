using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace taggie.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public ApplicationDbContext(ILoggerFactory loggerFactory, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.Property(e => e.EmailConfirmed)
                      .HasConversion<int>();

                entity.Property(e => e.LockoutEnabled)
                      .HasConversion<int>();

                entity.Property(e => e.PhoneNumberConfirmed)
                      .HasConversion<int>();

                entity.Property(e => e.TwoFactorEnabled)
                      .HasConversion<int>();
            });
        }
    }
}
