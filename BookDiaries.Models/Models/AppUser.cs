using BookDiaries.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.Models
{
    public class AppUser: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public string? Picture { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? StreetAddress { get; set; }
        public string? PostalCode { get; set; }

        [ValidateNever]
        public ICollection<UserCoupon>? UserCoupons { get; set; }
        [ValidateNever]
        public List<RatingReview> RatingReviews { get; set; }
    }
}
