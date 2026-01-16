using LoggAutorz.Users;
using Microsoft.EntityFrameworkCore;
namespace LoggAutorz.DataBase

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<UsersEntity> UserAccounts { get; set; }
    }
}
