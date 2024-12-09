using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesi.Models
{
    public class BitirmeProjesiiContext : IdentityDbContext<IdentityUser>
    {
        public BitirmeProjesiiContext(DbContextOptions<BitirmeProjesiiContext> options)
            : base(options)
        {
        }

        public DbSet<PollutionData> PollutionDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PollutionData>()
                .HasOne(p => p.EnteredByUser)
                .WithMany()
                .HasForeignKey(p => p.EnteredById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PollutionData>()
                .Property(p => p.Latitude)
                .HasColumnType("float(53)");  // SQL Server'da double precision float

            modelBuilder.Entity<PollutionData>()
                .Property(p => p.Longitude)
                .HasColumnType("float(53)");  // SQL Server'da double precision float

            modelBuilder.Entity<PollutionData>()
                .Property(p => p.Value)
                .HasColumnType("float(53)");  // SQL Server'da double precision float
        }
    }
}