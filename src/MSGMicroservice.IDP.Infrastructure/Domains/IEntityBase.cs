namespace MSGMicroservice.IDP.Infrastructure.Domains;

public interface IEntityBase<T>
{
    T Id { get; set; }
}