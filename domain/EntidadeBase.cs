namespace FinanceiroApi.Domain;

abstract class EntidadeBase<TKey> : IEntidade<TKey>
{
    public required TKey Id {get; init;}
}