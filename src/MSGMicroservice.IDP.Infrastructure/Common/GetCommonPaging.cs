namespace MSGMicroservice.IDP.Infrastructure.Common
{
    public class GetCommonPaging: PagingRequestBase
    {
        public string? Keyword { get; set; }
        public string? Filter1 { get; set; }
        public string? Filter2 { get; set; }
        public string? Filter3 { get; set; }
    }
}