namespace FinanceiroApi.Domain;

interface IRepositorioTransacao
{
    Task AdicionarAsync(Transacao transacao);
    Task<IReadOnlyCollection<Transacao>> FiltrarPorUsuarioAsync(Guid usuarioId);
}
