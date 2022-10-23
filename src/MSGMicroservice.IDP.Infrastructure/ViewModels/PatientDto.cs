using System;

namespace MSGMicroservice.IDP.Infrastructure.ViewModels
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime Dob { get; set; }
        public int? HospitalId { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? JobId { get; set; }
        public string Avatar { get; set; }
        public int? Nationality { get; set; }
        public int? GenderId { get; set; }
        public int? PlaceOfBirthId { get; set; }
        public string IdentityNumber { get; set; }
        public string CitizenIdentificationNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}