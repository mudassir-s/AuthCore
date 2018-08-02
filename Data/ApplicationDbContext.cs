using Microsoft.EntityFrameworkCore;
using AuthCore.Models.Entities;

namespace AuthCore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }
    }
}