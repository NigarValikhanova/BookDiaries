using BookDiaries.DataAccess.Data;
using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using Microsoft.AspNetCore.Mvc;
using BookDiaries.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BookDiariesWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]


    public class CouponController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Coupon> objCouponList = _unitOfWork.Coupon.GetAll().ToList();
            return View(objCouponList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Coupon obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Coupon.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Coupon created successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            Coupon? CouponFromDb = _unitOfWork.Coupon.Get(u=>u.Id==Id);
            if (CouponFromDb == null)
            {
                return NotFound();
            }
            return View(CouponFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Coupon obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Coupon.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Coupon updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            Coupon? CouponFromDb = _unitOfWork.Coupon.Get(u=>u.Id==Id);
            if (CouponFromDb == null)
            {
                return NotFound();
            }
            return View(CouponFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? Id)
        {
            Coupon? obj = _unitOfWork.Coupon.Get(u => u.Id == Id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Coupon.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Coupon deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
