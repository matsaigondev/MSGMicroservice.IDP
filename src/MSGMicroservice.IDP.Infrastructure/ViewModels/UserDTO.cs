using System.Collections.Generic;

namespace MSGMicroservice.IDP.Infrastructure.ViewModels
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public IList<string>? Roles { get; set; }
        public string? Role { get; set; }
        public string? OldRole { get; set; }
        public IList<string>? OldRoles { get; set; }
        public int? HospitalId { get; set; }
    }
}