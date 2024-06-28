using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Library> Libraries { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
}
