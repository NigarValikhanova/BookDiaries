using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using BookDiaries.Models.UserModels;
using BookDiaries.Models.ViewModels;
using BookDiaries.Utility;
using BookDiariesWeb.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace BookDiariesWeb.Controllers
{
    public class BooksController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BooksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(double? minPrice = null, double? maxPrice = null)
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll().ToList();

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            ShopVM shopVM = new ShopVM
            {
                CategoryList = _unitOfWork.Category.GetAll(),
                LanguageList = _unitOfWork.Language.GetAll(),
                AuthorList = _unitOfWork.Author.GetAll(),
                ProductList = products.ToList(),
                ProductImageList = _unitOfWork.ProductImage.GetAll(),
                ReviewList = _unitOfWork.RatingReview.GetAll()
            };

            return View(shopVM);
        }

        public IActionResult BestSeller()
        {
            // Retrieve only bestseller products from the repository
            var bestSellerProducts = _unitOfWork.Product.GetAll().Where(p => p.IsBestSeller);

            // Populate the view model with bestseller products
            ShopVM shopVM = new ShopVM
            {
                CategoryList = _unitOfWork.Category.GetAll(),
                LanguageList = _unitOfWork.Language.GetAll(),
                AuthorList = _unitOfWork.Author.GetAll(),
                ProductList = bestSellerProducts.ToList(),
                ProductImageList = _unitOfWork.ProductImage.GetAll(),
                ReviewList = _unitOfWork.RatingReview.GetAll()

            };

            return View(shopVM);
        }

        public IActionResult DealsPage()
        {
            // Retrieve only bestseller products from the repository
            var bestSellerProducts = _unitOfWork.Product.GetAll().Where(p => p.IsDealOfTheDay);

            // Populate the view model with bestseller products
            ShopVM shopVM = new ShopVM
            {
                CategoryList = _unitOfWork.Category.GetAll(),
                LanguageList = _unitOfWork.Language.GetAll(),
                AuthorList = _unitOfWork.Author.GetAll(),
                ProductList = bestSellerProducts.ToList(),
                ProductImageList = _unitOfWork.ProductImage.GetAll(),
                ReviewList = _unitOfWork.RatingReview.GetAll()

            };

            return View(shopVM);
        }

        #region Language
        public IActionResult GetBooksByLanguage(string languageName)
        {
            // Retrieve only bestseller products for the specified language from the repository
            var books = _unitOfWork.Product.GetAll().Where(p => p.Language.Name == languageName);

            ShopVM shopVM = new ShopVM
            {
                CategoryList = _unitOfWork.Category.GetAll(),
                LanguageList = _unitOfWork.Language.GetAll(),
                AuthorList = _unitOfWork.Author.GetAll(),
                ProductList = books.ToList(),
                ProductImageList = _unitOfWork.ProductImage.GetAll()
            };

            return View(shopVM);
        }
        public IActionResult Azerbaijani()
        {
            return GetBooksByLanguage("Azerbaijani");
        }

        public IActionResult English()
        {
            return GetBooksByLanguage("English");
        }

        public IActionResult Turkish()
        {
            return GetBooksByLanguage("Turkish");
        }

        public IActionResult Russian()
        {
            return GetBooksByLanguage("Russian");
        }
        #endregion

        #region Category
        public IActionResult GetProductsByCategory(string categoryName, double? minPrice = null, double? maxPrice = null)
        {
            var products = _unitOfWork.Product.GetAll().Where(p => p.Category.Name == categoryName);

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            var shopVM = new ShopVM
            {
                CategoryList = _unitOfWork.Category.GetAll(),
                LanguageList = _unitOfWork.Language.GetAll(),
                AuthorList = _unitOfWork.Author.GetAll(),
                ProductList = products.ToList(),
                ProductImageList = _unitOfWork.ProductImage.GetAll(),
                ReviewList = _unitOfWork.RatingReview.GetAll()
            };

            return View(shopVM);
        }

        public IActionResult Manga()
        {
            return GetProductsByCategory("Manga");
        }
        public IActionResult Comics()
        {
            return GetProductsByCategory("Comics");
        }
        public IActionResult Fantasy()
        {
            return GetProductsByCategory("Fantasy");
        }
        public IActionResult Romance()
        {
            return GetProductsByCategory("Romance");
        }
        public IActionResult Thriller()
        {
            return GetProductsByCategory("Thriller");
        }
        public IActionResult SciFi()
        {
            return GetProductsByCategory("SciFI");
        }
        public IActionResult Action()
        {
            return GetProductsByCategory("Action");
        }
        public IActionResult Mystery()
        {
            return GetProductsByCategory("Mystery");
        }
        public IActionResult Tragedy()
        {
            return GetProductsByCategory("Tragedy");
        }

        #endregion

        public IActionResult BookDetails(int Id)
        {
            var product = _unitOfWork.Product.Get(u => u.Id == Id, includeProperties: "Category,Author,Language,ProductImages");
            // Ensure that AppUser is included when fetching reviews to access user details like FirstName, LastName, and Picture
            var reviews = _unitOfWork.RatingReview.GetAll(
                filter: r => r.ProductId == Id,
                includeProperties: "AppUser" // Include AppUser to load user details for each review
            ).ToList();

            var viewModel = new BookDetailsVM
            {
                ShoppingCart = new ShoppingCart
                {
                    Product = product,
                    ProductId = Id,
                    Count = 1
                },
                Reviews = reviews,
                NewReview = new RatingReview() // Optionally prepare a new review model for submission
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.AppUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.AppUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                //add cart record
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId).Count());
            }
            TempData["success"] = "Cart updated successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToWishlist(WishList wishList)
        {
            wishList.Id = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            wishList.AppUserId = userId;

            WishList wishFromDb = _unitOfWork.WishList.Get(u => u.AppUserId == userId &&
            u.ProductId == wishList.ProductId);

            if (wishFromDb != null)
            {
                //shopping cart exists
                wishFromDb.Count += wishList.Count;
                _unitOfWork.WishList.Update(wishFromDb);
                _unitOfWork.Save();
            }
            else
            {
                //add cart record
                _unitOfWork.WishList.Add(wishList);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId).Count());

            }
            TempData["success"] = "Product added successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddReview(BookDetailsVM model)
        {
            var review = model.NewReview;
            review.AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Make sure the User ID is set correctly based on your authentication system

            _unitOfWork.RatingReview.Add(review);
            _unitOfWork.Save();

            return RedirectToAction("BookDetails", new { id = review.ProductId });
        }

        public IActionResult Search(string query)
        {

            // Safely perform the search operation, considering potential null values in Title and Author.Name
            var books = _unitOfWork.Product.GetAll(includeProperties: "Category,Language,Author")
                             .Where(p => (p.Title != null && p.Title.Contains(query)) ||
                                         (p.Author != null && !string.IsNullOrEmpty(p.Author.Name) && p.Author.Name.Contains(query)))
                             .ToList();

            // Prepare the ViewModel to pass to the View
            ShopVM shopVM = new ShopVM
            {
                CategoryList = _unitOfWork.Category.GetAll(),
                LanguageList = _unitOfWork.Language.GetAll(),
                AuthorList = _unitOfWork.Author.GetAll(),
                ProductList = books,
                ProductImageList = _unitOfWork.ProductImage.GetAll(),
                ReviewList = _unitOfWork.RatingReview.GetAll()

            };

            // Return the view with the ViewModel
            return View(shopVM);
        }


    }
}
