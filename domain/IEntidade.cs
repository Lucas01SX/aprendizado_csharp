namespace FinanceiroApi.Domain;

interface IEntidade<TKey>
{
    TKey Id { get; }
}