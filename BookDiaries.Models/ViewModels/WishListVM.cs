using BookDiaries.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.ViewModels
{
    public class WishListVM
    {
        public IEnumerable<WishList> WishesLists { get; set; }
        public IEnumerable<ProductImage> ProductImageLists { get; set; }
        public OrderHeader OrderHeader { get; set; }

    }
}
