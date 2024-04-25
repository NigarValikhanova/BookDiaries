using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookDiariesWeb.ViewComponents
{
    public class WishListViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public WishListViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionWish) != null)
                {
                    HttpContext.Session.SetInt32(SD.SessionWish,
                        _unitOfWork.WishList.GetAll(u => u.AppUserId == claim.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(SD.SessionWish));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
