using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using MSGMicroservice.IDP.Infrastructure.Domains;

namespace MSGMicroservice.IDP.Infrastructure.Entities
{
    public class Permission : EntityBase<long>
    {
        public Permission()
        {
        }

        public Permission(string function, string command, string roleId)
        {
            Function = function;
            Command = command;
            RoleId = roleId;
        }

        [Key]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Function { get; set; }

        [Key]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Command { get; set; }

        [ForeignKey("RoleId")] public string RoleId { get; set; }

        public virtual IdentityRole Role { get; set; }
    }
}