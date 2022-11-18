namespace MSGMicroservice.IDP.Infrastructure.Common
{
    public class GetCommonPaging: PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
}