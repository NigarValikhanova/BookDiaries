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

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category,Language,Author").ToList();
            return View(objProductList);
        }
        public IActionResult Upsert(int? Id)
        {
            ProductVM productVM = new()
            {
                AuthorList = _unitOfWork.Author.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                LanguageList = _unitOfWork.Language.GetAll().Select(z => new SelectListItem
                {
                    Text = z.Name,
                    Value = z.Id.ToString()
                }),
                Product = new Product()
            };
            if (Id == null || Id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == Id, includeProperties: "ProductImages");
                return View(productVM);
            }
        }

        //[HttpPost]
        //public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (productVM.Product.Id == 0)
        //        {
        //            _unitOfWork.Product.Add(productVM.Product);
        //        }
        //        else
        //        {
        //            _unitOfWork.Product.Update(productVM.Product);
        //        }

        //        _unitOfWork.Save();


        //        string wwwRootPath = _webHostEnvironment.WebRootPath;
        //        if (files != null)
        //        {

        //            foreach (IFormFile file in files)
        //            {
        //                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        //                string productPath = @"admin\images\products\product-" + productVM.Product.Id;
        //                string finalPath = Path.Combine(wwwRootPath, productPath);

        //                if (!Directory.Exists(finalPath))
        //                    Directory.CreateDirectory(finalPath);

        //                using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
        //                {
        //                    file.CopyTo(fileStream);
        //                }

        //                ProductImage productImage = new()
        //                {
        //                    ImageUrl = productPath + @"\" + fileName,
        //                    ProductId = productVM.Product.Id,
        //                };

        //                if (productVM.Product.ProductImages == null)
        //                    productVM.Product.ProductImages = new List<ProductImage>();

        //                productVM.Product.ProductImages.Add(productImage);

        //            }

        //            _unitOfWork.Product.Update(productVM.Product);
        //            _unitOfWork.Save();
        //        }
        //        TempData["success"] = "Product created/updated successfully";
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        });
        //        productVM.LanguageList = _unitOfWork.Language.GetAll().Select(z => new SelectListItem
        //        {
        //            Text = z.Name,
        //            Value = z.Id.ToString()
        //        });
        //        productVM.AuthorList = _unitOfWork.Author.GetAll().Select(x => new SelectListItem
        //        {
        //            Text = x.Name,
        //            Value = x.Id.ToString()
        //        });

        //        return View(productVM);
        //    }
        //}

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFileCollection files = null)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    // Block for Adding a new product
                    _unitOfWork.Product.Add(productVM.Product);
                    _unitOfWork.Save(); // Save the new product first to ensure it has an ID for image processing

                    if (files != null && files.Count > 0)
                    {
                        ProcessProductImages(files, productVM.Product.Id);
                    }
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    // Block for Updating an existing product
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save(); // Save changes to the existing product

                    if (files != null && files.Count > 0)
                    {
                        ProcessProductImages(files, productVM.Product.Id);
                    }
                    TempData["success"] = "Product updated successfully";
                }

                _unitOfWork.Save(); // Save any changes or added images to the database
                return RedirectToAction("Index");
            }
            else
            {
                // If model state is not valid, re-populate the lists for dropdowns and return to the form
                ReinitializeViewModel(productVM);
                return View(productVM);
            }
        }

        private void ProcessProductImages(IFormFileCollection files, int productId)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string productPath = Path.Combine(wwwRootPath, "admin", "images", "products", $"product-{productId}");
            Directory.CreateDirectory(productPath); // Create directory if it does not exist

            foreach (IFormFile file in files)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(productPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                ProductImage productImage = new ProductImage
                {
                    ImageUrl = $"admin/images/products/product-{productId}/{fileName}",
                    ProductId = productId
                };

                _unitOfWork.ProductImage.Add(productImage);
            }
        }

        private void ReinitializeViewModel(ProductVM productVM)
        {
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            productVM.LanguageList = _unitOfWork.Language.GetAll().Select(z => new SelectListItem
            {
                Text = z.Name,
                Value = z.Id.ToString()
            });
            productVM.AuthorList = _unitOfWork.Author.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }


        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                                   Path.Combine(_webHostEnvironment.WebRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category,Language,Author").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == Id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string productPath = @"admin\images\products\product-" + Id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }


            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });

        }


        #endregion

    }
}
