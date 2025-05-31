using Document_Intelligence_Task.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Document_Intelligence_Task.Data
{
    public class DocumentIntelligenceDB : DbContext
    {
        public DocumentIntelligenceDB(DbContextOptions<DocumentIntelligenceDB> options) : base(options) { }
        public DbSet<IDDocument_Passport> Passports{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
