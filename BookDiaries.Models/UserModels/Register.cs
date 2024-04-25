using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.UserModels
{
    public class Register
    {
        [Required(ErrorMessage = "Istifadechi adi bosh ola bilmez")]
        [Display(Name = "Username")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Email formati duzgun qeyd edilmeyib")]
        [Required(ErrorMessage = "Email hissesi bosh ola bilmez")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Shifre bosh ola bilmez")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Tekrar Shifre bosh ola bilmez")]
        [Display(Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Shifreler eyni deyil")]
        public string? ConfirmPassword { get; set; }
        public string? Role { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
