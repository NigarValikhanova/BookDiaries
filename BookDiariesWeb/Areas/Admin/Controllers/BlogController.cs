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

    public class BlogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Blog> objBlogList = _unitOfWork.Blog.GetAll().ToList();
            return View(objBlogList);
        }
        public IActionResult Upsert(int? Id)
        {
            Blog blog = new Blog();

            if (Id == null || Id == 0)
            {
                //create
                return View(blog);
            }
            else
            {
                //update
                blog = _unitOfWork.Blog.Get(u => u.Id == Id);
                return View(blog);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Blog blog, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (blog.Id == 0)
                {
                    blog.CreatedAt = DateTime.Today; // Set the current date
                }
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"admin\images\blog");

                    if (!string.IsNullOrEmpty(blog.ImageUrl))
                    {
                        //Delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, blog.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    blog.ImageUrl = @"admin\images\blog\" + fileName;

                }
                if (blog.Id == 0)
                {
                    _unitOfWork.Blog.Add(blog);
                    TempData["success"] = "Blog created successfully";
                }
                else
                {
                    _unitOfWork.Blog.Update(blog);
                    TempData["success"] = "Blog updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View(blog);
            }
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Blog> objBlogList = _unitOfWork.Blog.GetAll().ToList();
            return Json(new { data = objBlogList });
        }

        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            var blogToBeDeleted = _unitOfWork.Blog.Get(u => u.Id == Id);
            if (blogToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, blogToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Blog.Remove(blogToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });

        }


        #endregion

    }
}
