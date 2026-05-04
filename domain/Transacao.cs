namespace FinanceiroApi.Domain;

class Transacao : EntidadeBase<Guid>
{
    public required string   Descricao  { get; set; }
    public required decimal  Valor      { get; set; }
    public required DateTime Data       { get; set; }
    public required Guid     UsuarioId  { get; set; }
}
