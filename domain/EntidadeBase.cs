namespace FinanceiroApi.Domain;

abstract class EntidadeBase<TKey> : IEntidade<TKey>, IEquatable<EntidadeBase<TKey>>
{
    protected EntidadeBase() {}

    public TKey Id {get; init;} = default!; 

    public bool Equals(EntidadeBase<TKey>? other)
    {
        if (other is null) return false;
        
        if (ReferenceEquals(this, other)) return true;

        return EqualityComparer<TKey>.Default.Equals(Id, other.Id); 
    }
    public override bool Equals(object? obj)
    {
        if (obj is EntidadeBase<TKey> other)
        {
            return Equals(other);
        }
        return false;
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        return Id is null ? 0 : EqualityComparer<TKey>.Default.GetHashCode(Id);
    }
}