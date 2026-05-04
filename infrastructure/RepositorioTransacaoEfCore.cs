using FinanceiroApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceiroApi.Infrastructure;

class RepositorioTransacaoEfCore(AppDbContext context) : IRepositorioTransacao
{
    public async Task AdicionarAsync(Transacao transacao)
    {
        context.Transacoes.Add(transacao);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<Transacao>> FiltrarPorUsuarioAsync(Guid usuarioId)
    {
        return await context.Transacoes
            .Where(t => t.UsuarioId == usuarioId)
            .ToListAsync();
    }
}
