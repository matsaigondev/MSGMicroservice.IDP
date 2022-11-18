namespace MSGMicroservice.IDP.Infrastructure.Common
{
    public class PagingRequestBase
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
        public string? DomainId { get; set; }
        public string? UserId { get; set; }
        public string? ParentId { get; set; }
        public string? StatusId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? SubId { get; set; }
        public string? RoleName { get; set; }
    }
}