
using FinanceiroApi.Domain;

namespace FinanceiroApi.Infrastructure;

class RepositorioUsuarioMemoria : IRepositorioUsuario
{
    private Dictionary<Guid, Usuario> _usuarios = new();

    public void Adicionar(Usuario usuario)
    {

        if(!_usuarios.ContainsKey(usuario.Id))
        {
            _usuarios[usuario.Id] = usuario;
        }
    }

    public Usuario? ObterPorId(Guid id)
    {
        _usuarios.TryGetValue(id, out Usuario? usuario);
        return usuario;
    }
    public IReadOnlyCollection<Usuario> ProcessarLista(List<Usuario> listaBruta)
    {
        HashSet<Usuario> usuariosUnicos = new HashSet<Usuario>(listaBruta);
        return usuariosUnicos;   
    } 
}