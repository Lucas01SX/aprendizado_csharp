using FinanceiroApi.Domain;

namespace FinanceiroApi.Infrastructure;

class RepositorioUsuarioEfCore(AppDbContext context) : IRepositorioUsuario
{
    public void Adicionar(Usuario usuario)
    {
        context.Usuarios.Add(usuario);
        context.SaveChanges();
    }
    public Usuario? ObterPorId(Guid id)
    {
        return context.Usuarios.Find(id);
    }
    public IReadOnlyCollection<Usuario> ProcessarLista(List<Usuario> usuarios)
    {
        HashSet<Usuario> usuariosUnicos = new HashSet<Usuario>(usuarios);
        return usuariosUnicos;   
    }
    public IReadOnlyCollection<Usuario> Filtrar(Func<Usuario, bool> filtro)
    {
        return context.Usuarios.Where(filtro).ToList();
    }
}