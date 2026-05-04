using FinanceiroApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceiroApi.Infrastructure;


class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario>    Usuarios    { get; set; } = null!;
    public DbSet<Transacao>  Transacoes  { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transacao>()
            .Property(t => t.Valor)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Transacoes)
            .WithOne()
            .HasForeignKey(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}