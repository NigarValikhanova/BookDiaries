using BookDiaries.Models.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookDiaries.Models.ViewModels
{
    public class AdminVM
    {
        [ValidateNever]
        public IEnumerable<Product> ProductList { get; set; }

        [ValidateNever]
        public IEnumerable<Category> CategoryList { get; set; }
        [ValidateNever]
        public IEnumerable<Language> LanguageList { get; set; }
        [ValidateNever]
        public IEnumerable<Author> AuthorList { get; set; }
        [ValidateNever]
        public IEnumerable<OrderHeader> OrderList { get; set; }
        [ValidateNever]
        public IEnumerable<AppUser> UserList { get; set; }
    }
}
