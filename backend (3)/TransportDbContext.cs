

using Microsoft.EntityFrameworkCore;

namespace Transport.Data
{
    public class TransportDbContext :DbContext
    {
        public TransportDbContext(DbContextOptions<TransportDbContext> options) :base(options)
        {
            
        }
        public DbSet<UserDetail> userDetail { get; set; }  
    }
}
