using System;
using System.Collections.Generic;
using System.Text;
using LogBasePresenter.DatabaseModels;
using LogBasePresenter.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LogBasePresenter
{
    class LogBaseContext : DbContext
    {
#if DEBUG
        public LogBaseContext() : base(new DbContextOptionsBuilder().UseNpgsql("Server=127.0.0.1;Port=5432;Database=EttudItis20Test;User Id=postgres;Password=postgres;").Options) { }
#endif
        public LogBaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogRecord>(e =>
            {
                e.Property(p => p.QueryDescription).HasColumnType("json");
            });

            modelBuilder.Entity<Subnet>(e =>
            {
                e.HasOne(s => s.Country)
                    .WithMany(c => c.Subnets)
                    .HasForeignKey(d => d.CountryId)
                    .IsRequired();
            });
        }

        public DbSet<LogRecord> LogRecords { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Subnet> Subnet { get; set; }
    }
}
