using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MSGMicroservice.IDP.Infrastructure.Entities
{
    public class User : IdentityUser
    {
        [Column(TypeName = "nvarchar(150)")]
        public string FirstName { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public int? HospitalId { get; set; }
    }
}