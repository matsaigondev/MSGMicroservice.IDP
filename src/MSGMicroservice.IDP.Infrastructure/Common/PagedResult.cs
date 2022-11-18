using System.Collections.Generic;

namespace MSGMicroservice.IDP.Infrastructure.Common
{
    public class PagedResult<T> : PagedResultBase
    {
        public List<T> Data { set; get; }
    }
}