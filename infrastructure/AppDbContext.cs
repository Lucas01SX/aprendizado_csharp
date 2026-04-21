using FinanceiroApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceiroApi.Infrastructure;


class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios { get; set;} = null!;
}