namespace FinanceiroApi.Domain;

interface IRepositorioUsuario
{
    public void Adicionar(Usuario usuario);
    public Usuario? ObterPorId(Guid id);
    public IReadOnlyCollection<Usuario> ProcessarLista(List<Usuario> listaBruta);
    public IReadOnlyCollection<Usuario> Filtrar(Func<Usuario, bool> filtro);
}