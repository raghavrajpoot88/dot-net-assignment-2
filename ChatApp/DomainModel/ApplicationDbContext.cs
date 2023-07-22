using ChatApp.DomainModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ChatApp.DomainModel
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {

        }
        public DbSet<User> users { get; set; }
        public DbSet<Messages> messages { get; set; }


    }
}
