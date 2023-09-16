using Microsoft.EntityFrameworkCore;
using QLBanPhanMem.Models;
namespace ContactWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<AccountModel> Accounts { get; set; }
    }
}