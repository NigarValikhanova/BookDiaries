using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.UserModels
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Kohne shifre bosh ola bilmez")]
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Yeni shifre bosh ola bilmez")]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Tekrar shifre bosh ola bilmez")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Shifreler eyni deyil")]
        public string? ConfirmPassword { get; set; }
    }
}
