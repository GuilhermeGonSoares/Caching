using Api.Entities;
using Api.Interfaces.UnitOfWork;
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
        base.OnModelCreating(modelBuilder);

        // Mapeamento para a entidade Library
        modelBuilder.Entity<Library>().HasKey(l => l.Id); // Definindo a chave primária

        modelBuilder.Entity<Library>().Property(l => l.Id).ValueGeneratedOnAdd(); // Configura ID para autoincremento

        modelBuilder.Entity<Library>().Property(l => l.Name).IsRequired(); // Define o nome como obrigatório

        modelBuilder
            .Entity<Library>()
            .HasMany(l => l.Books) // Relacionamento um-para-muitos com Books
            .WithOne() // Sem propriedade de navegação inversa especificada
            .HasForeignKey("LibraryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        // Chave estrangeira em Books referenciando Library

        // Mapeamento para a entidade Book
        modelBuilder.Entity<Book>().HasKey(b => b.Id); // Definindo a chave primária

        modelBuilder.Entity<Book>().Property(b => b.Id).ValueGeneratedOnAdd(); // Configura ID para autoincremento

        modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired(); // Título é obrigatório

        modelBuilder.Entity<Book>().Property(b => b.Author).IsRequired(); // Autor é obrigatório

        modelBuilder.Entity<Book>().Property(b => b.IsAvailable).IsRequired(); // Disponibilidade é obrigatório, define como um campo não nulo
    }

    public void LogChanges(ApplicationDbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries())
        {
            Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
        }
    }
}
