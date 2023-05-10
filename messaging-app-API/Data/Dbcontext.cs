using messaging_app_API.Models;
using Microsoft.EntityFrameworkCore;

namespace messaging_app_API.Data
{
    public class Dbcontext : DbContext
    {
        public Dbcontext(DbContextOptions option) : base(option)
        {

        }

        public DbSet<User> Users { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{ 
        //}
    }

}