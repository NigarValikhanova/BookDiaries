using BookDiaries.DataAccess.Data;
using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using BookDiaries.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookDiaries.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BookDiariesWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class SliderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SliderController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Slider> objSliderList = _unitOfWork.Slider.GetAll().ToList();
            return View(objSliderList);
        }
        public IActionResult Upsert(int? Id)
        {
            Slider slider = new Slider();
            
            if(Id==null|| Id == 0)
            {
                //create
                return View(slider);
            }
            else
            {
                //update
                slider = _unitOfWork.Slider.Get(u => u.Id == Id);
                return View(slider);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Slider slider, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    string fileName = Guid.NewGuid().ToString() +Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"admin\images\slider");

                    if(!string.IsNullOrEmpty(slider.ImageUrl))
                    {
                        //Delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, slider.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    slider.ImageUrl = @"admin\images\slider\" + fileName;

                }
                if (slider.Id == 0)
                {
                    _unitOfWork.Slider.Add(slider);
                    TempData["success"] = "Slider created successfully";
                }
                else
                {
                    _unitOfWork.Slider.Update(slider);
                    TempData["success"] = "Slider updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View(slider);
            }
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Slider> objSliderList = _unitOfWork.Slider.GetAll().ToList();
            return Json(new { data = objSliderList });
        }

        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            var sliderToBeDeleted = _unitOfWork.Slider.Get(u => u.Id == Id);
            if (sliderToBeDeleted == null)
            {
                return Json(new {success = false, message = "Error while deleting"});
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, sliderToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Slider.Remove(sliderToBeDeleted);
            _unitOfWork.Save();
            
            return Json(new {success = true, message = "Delete Successful"});

        }


        #endregion

    }
}
