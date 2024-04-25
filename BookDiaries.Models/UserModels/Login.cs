using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.UserModels
{
    public class Login
    {
        [Required(ErrorMessage = "Email hissesi bosh ola bilmez")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email formati duzgun qeyd edilmeyib")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Shifre bosh ola bilmez")]
        [Display(Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
