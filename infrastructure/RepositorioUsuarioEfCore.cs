using FinanceiroApi.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinanceiroApi.Infrastructure;

class RepositorioUsuarioEfCore(AppDbContext context) : IRepositorioUsuario
{
    public async Task AdicionarAsync(Usuario usuario)
    {
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
    }
    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        return await context.Usuarios.FindAsync(id);
    }
    public Task<IReadOnlyCollection<Usuario>> ProcessarListaAsync(List<Usuario> usuarios)
    {
        IReadOnlyCollection<Usuario> resultado = new HashSet<Usuario>(usuarios);
        return Task.FromResult(resultado);   
    }
    public async Task<IReadOnlyCollection<Usuario>> FiltrarAsync(Expression<Func<Usuario, bool>> filtro)
    {
        return await context.Usuarios.Where(filtro).ToListAsync();
    }
}