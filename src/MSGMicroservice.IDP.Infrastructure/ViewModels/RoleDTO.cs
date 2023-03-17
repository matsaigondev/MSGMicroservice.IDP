using System;
using System.Collections.Generic;

namespace MSGMicroservice.IDP.Infrastructure.ViewModels
{
    public class RoleDTO
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public List<string>? Functions { get; set; }
    }
}