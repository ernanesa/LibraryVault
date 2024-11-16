using LibraryVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryVault.Infrastructure.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().OwnsOne(b => b.ISBN);
        }
    }

}