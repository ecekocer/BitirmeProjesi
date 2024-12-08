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

        // Kendi tablolarınızı burada tanımlayabilirsiniz
        public DbSet<PollutionData> PollutionDatas { get; set; }
                 //public DbSet<Product> Products { get; set; }
        // public DbSet<Customer> Customers { get; set; }
    }
}