using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSGMicroservice.IDP.Infrastructure.ViewModels
{
    public class ChangePwdDto
    {
        public string UserName { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "Old Password Required")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "New Password Required")]
        public string NewPassword { get; set; }
    }
}
