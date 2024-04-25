using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.Models
{
    public class RatingReview
    {
        public int Id { get; set; }

        [Required]
        public int Rating { get; set; }  // Rating (e.g., 1 to 5 stars)

        [Required]
        public string Review { get; set; }  // Review text

        public string AppUserId { get; set; }  // User who left the review
        [ForeignKey("AppUserId")]
        [ValidateNever]
        public AppUser AppUser { get; set; }
        public int ProductId { get; set; }  // Product being reviewed
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }

    }

}
