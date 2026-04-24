using System.Linq.Expressions;
using FinanceiroApi.Domain;

namespace FinanceiroApi.Infrastructure;

class RepositorioUsuarioMemoria : IRepositorioUsuario
{
    private Dictionary<Guid, Usuario> _usuarios = new();

    public Task AdicionarAsync(Usuario usuario)
    {
        if (!_usuarios.ContainsKey(usuario.Id))
            _usuarios[usuario.Id] = usuario;
        return Task.CompletedTask;
    }

    public Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        _usuarios.TryGetValue(id, out Usuario? usuario);
        return Task.FromResult(usuario);
    }

    public Task<IReadOnlyCollection<Usuario>> ProcessarListaAsync(List<Usuario> listaBruta)
    {
        IReadOnlyCollection<Usuario> resultado = new HashSet<Usuario>(listaBruta);
        return Task.FromResult(resultado);
    }

    public Task<IReadOnlyCollection<Usuario>> FiltrarAsync(Expression<Func<Usuario, bool>> filtro)
    {
        IReadOnlyCollection<Usuario> resultado = _usuarios.Values.Where(filtro.Compile()).ToList();
        return Task.FromResult(resultado);
    }
}