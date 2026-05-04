namespace FinanceiroApi.Domain;

class Usuario : EntidadeBase<Guid>
{
    public required string Nome  { get; set; }
    public required string Email { get; set; }
    public ICollection<Transacao> Transacoes { get; set; } = [];
}