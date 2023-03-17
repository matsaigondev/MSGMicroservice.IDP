namespace MSGMicroservice.IDP.Infrastructure.ViewModels
{
    public class PermissionRoleVm
    {
        public string Function { get; set; }
        public bool View { get; set; }
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool Import { get; set; }
        public bool Export { get; set; }
        public bool Show { get; set; }
        public bool Print { get; set; }
        public string RoleId { get; set; }
    }
}