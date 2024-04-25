using BookDiaries.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.ViewModels
{
    public class BookDetailsVM
    {
        public ShoppingCart ShoppingCart { get; set; }
        public IEnumerable<RatingReview> Reviews { get; set; }
        public RatingReview NewReview { get; set; } = new RatingReview();
    }
}
