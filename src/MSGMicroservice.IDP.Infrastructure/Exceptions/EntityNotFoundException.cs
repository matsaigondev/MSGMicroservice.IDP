using System;

namespace MSGMicroservice.IDP.Infrastructure.Exceptions
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException() : base("Entity was not found.")
        {
        }
    }
}