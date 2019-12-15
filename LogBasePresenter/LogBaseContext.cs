using System;
using System.Collections.Generic;
using System.Text;
using LogBasePresenter.Models;
using Microsoft.EntityFrameworkCore;

namespace LogBasePresenter
{
    class LogBaseContext : DbContext
    {
        public LogBaseContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=EttudItis20Test;User Id=postgres;Password=postgres;");
                /*#if DEBUG
                                optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole(LogLevel.Debug));
                #endif*/
            }
        }

        public DbSet<LogRecord> LogRecords { get; set; }
    }
}
