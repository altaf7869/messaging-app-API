using LegalGen.Models;
using Microsoft.EntityFrameworkCore;

namespace LegalGen.Data
{
    public class Dbcontext : DbContext
    {
        public Dbcontext(DbContextOptions option) : base(option)
        {

        }

        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)


        { 
        }
    }

}