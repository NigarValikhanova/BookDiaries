using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using BookDiaries.Models.ViewModels;
using BookDiaries.Utility;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookDiariesWeb.Controllers
{
    public class WishListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public WishListVM WishListVM { get; set; }
        public WishListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            WishListVM = new()
            {
                WishesLists = _unitOfWork.WishList.GetAll(u => u.AppUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new(),
                ProductImageLists = _unitOfWork.ProductImage.GetAll()
            };

            foreach (var wish in WishListVM.WishesLists)
            {
                wish.Price = GetPriceBasedOnQuantity(wish);
                WishListVM.OrderHeader.OrderTotal += (wish.Price * wish.Count);
            }

            return View(WishListVM);
        }


        public IActionResult Plus(int wishId)
        {
            var wishFromDb = _unitOfWork.WishList.Get(u => u.Id == wishId);
            wishFromDb.Count += 1;
            _unitOfWork.WishList.Update(wishFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int wishId)
        {
            var wishFromDb = _unitOfWork.WishList.Get(u => u.Id == wishId, tracked: true);
            if (wishFromDb.Count <= 1)
            {
                //remove that from wishlist
                HttpContext.Session.SetInt32(SD.SessionWish, _unitOfWork.WishList
                    .GetAll(u => u.AppUserId == wishFromDb.AppUserId).Count() - 1);
                _unitOfWork.WishList.Remove(wishFromDb);
            }
            else
            {
                wishFromDb.Count -= 1;
                _unitOfWork.WishList.Update(wishFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int wishId)
        {
            var wishFromDb = _unitOfWork.WishList.Get(u => u.Id == wishId, tracked: true);
            HttpContext.Session.SetInt32(SD.SessionWish, _unitOfWork.WishList
               .GetAll(u => u.AppUserId == wishFromDb.AppUserId).Count() - 1);
            _unitOfWork.WishList.Remove(wishFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(WishList wishList)
        {
            if (wishList.Count <= 50)
            {
                return wishList.Product.Price;
            }
            else
            {
                if (wishList.Count <= 100)
                {
                    return wishList.Product.Price50;
                }
                else
                {
                    return wishList.Product.Price100;
                }
            }
        }
    }
}
