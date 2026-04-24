using System.Linq.Expressions;

namespace FinanceiroApi.Domain;

interface IRepositorioUsuario
{
    public Task AdicionarAsync(Usuario usuario);
    public Task<Usuario?> ObterPorIdAsync(Guid id);
    public Task<IReadOnlyCollection<Usuario>> ProcessarListaAsync(List<Usuario> listaBruta);
    public Task<IReadOnlyCollection<Usuario>> FiltrarAsync(Expression<Func<Usuario, bool>> filtro);
}