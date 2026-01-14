using ApiLoggingExample.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ApiLoggingExample.Data
{
    public class LoggingDbContext : DbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options)
            : base(options) { }

        public DbSet<ApiLog> ApiLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiLog>().ToTable("ApiLogs", "dbo");
        }

    }

}
