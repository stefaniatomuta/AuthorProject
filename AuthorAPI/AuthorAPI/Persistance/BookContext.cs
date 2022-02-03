using AuthorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorAPI.Persistance {
    public class BookContext : DbContext {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source = Authors.db");
        }
    }
}