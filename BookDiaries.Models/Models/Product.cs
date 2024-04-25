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
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }
        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        public int CategoryId {  get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        public int LanguageId { get; set; }

        [ForeignKey("LanguageId")]
        [ValidateNever]
        public Language Language { get; set; }

        [Display(Name = "Is Bestseller")]
        [ValidateNever]
        public bool IsBestSeller { get; set; }

        [Display(Name = "Is Deal Of The Day?")]
        [ValidateNever]
        public bool IsDealOfTheDay { get; set; }

        public int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        [ValidateNever]
        public Author Author { get; set; }

        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
        [ValidateNever]
        public List<RatingReview> RatingReviews { get; set; }
    }
}
