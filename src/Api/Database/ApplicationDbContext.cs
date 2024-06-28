using Api.Entities;
using Api.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Api.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options),
        IUnitOfWork
{
    public DbSet<Library> Libraries { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Library>()
            .HasMany(l => l.Books)
            .WithOne()
            .HasForeignKey("LibraryId") // Apenas se você tiver uma propriedade LibraryId em Book
            .OnDelete(DeleteBehavior.Cascade); // Opção para deletar todos os livros quando uma biblioteca é deletada

        base.OnModelCreating(modelBuilder);
    }

    public void LogChanges(ApplicationDbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries())
        {
            Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
        }
    }
}
