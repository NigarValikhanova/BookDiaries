using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.UserModels
{
    public class EditUser
    {
        [Required(ErrorMessage = "Istifadechi adi bosh ola bilmez")]
        [Display(Name = "Username")]
        public string Username { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email formati duzgun qeyd edilmeyib")]
        [Required(ErrorMessage = "Email hissesi bosh ola bilmez")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Telefon nomresi bosh ola bilmez")]
        public string PhoneNumber { get; set; } = null!;
        [Required(ErrorMessage = "Ad bosh ola bilmez")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Soyad bosh ola bilmez")]
        public string LastName { get; set; } = null!;

        [DataType(DataType.Date)]
        public string? BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public IFormFile? Picture { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? StreetAddress { get; set; }
        public string? PostalCode { get; set; }
        public string? PictureUrl { get; set; }
    }
}
