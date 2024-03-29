﻿namespace MSGMicroservice.IDP.Infrastructure.ViewModels
{
    public class RegisterRequestDTO
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public List<string>? Roles { get; set; }
        public List<string>? OldRoles { get; set; }
        public int? HospitalId { get; set; }
    }
}