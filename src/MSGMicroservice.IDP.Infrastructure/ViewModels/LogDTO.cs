namespace MSGMicroservice.IDP.Infrastructure.ViewModels
{
    public class LogDTO
    {
        public Guid Id { get; set; }
        public int HospitalId { get; set; }
        public string HospitalName { get; set; }
        public string Category { get; set; }
        public string Event { get; set; }
        public string Content { get; set; }
        public string Result { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}