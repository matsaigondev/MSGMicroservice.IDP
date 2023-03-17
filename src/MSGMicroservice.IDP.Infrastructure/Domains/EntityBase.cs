using MSGMicroservice.IDP.Infrastructure.Domains;

namespace MSGMicroservice.IDP.Infrastructure.Domains
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        public TKey Id { get; set; }
    }
}