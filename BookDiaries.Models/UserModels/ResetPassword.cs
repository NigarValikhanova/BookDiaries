using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.UserModels
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "Shifre bosh ola bilmez")]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Tekrar Shifre bosh ola bilmez")]
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Shifreler eyni deyil")]
        public string? ConfirmPassword { get; set; }
    }
}
