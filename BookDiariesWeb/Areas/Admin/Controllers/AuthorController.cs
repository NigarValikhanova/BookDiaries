using BookDiaries.DataAccess.Data;
using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using BookDiaries.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BookDiariesWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class AuthorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Author> objAuthorList = _unitOfWork.Author.GetAll().ToList();
            return View(objAuthorList);
        }
        // GET: Author/Create
        public IActionResult Create()
        {
            return PartialView("_CreateAuthorPartial", new Author());
        }

        // POST: Author/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Author obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Author.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Author created successfully";
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_CreateAuthorPartial", obj);
        }

        // GET: Author/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var AuthorFromDb = _unitOfWork.Author.Get(u => u.Id == id);
            if (AuthorFromDb == null)
            {
                return NotFound();
            }
            return PartialView("_EditAuthorPartial", AuthorFromDb);
        }

        // POST: Author/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Author obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Author.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Author updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_EditAuthorPartial", obj);
        }

        // GET: Author/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var AuthorFromDb = _unitOfWork.Author.Get(u => u.Id == id);
            if (AuthorFromDb == null)
            {
                return NotFound();
            }
            return PartialView("_DeleteAuthorPartial", AuthorFromDb);
        }

        // POST: Author/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obj = _unitOfWork.Author.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Author.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Author deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
