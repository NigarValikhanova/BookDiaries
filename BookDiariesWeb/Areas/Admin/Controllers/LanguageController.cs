using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using Microsoft.AspNetCore.Mvc;
using BookDiaries.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace BookDiariesWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class LanguageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LanguageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var objLanguageList = _unitOfWork.Language.GetAll();
            return View(objLanguageList);
        }

        // GET: Language/Create
        public IActionResult Create()
        {
            return PartialView("_CreateLanguagePartial", new Language());
        }

        // POST: Language/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Language obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Language.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Language created successfully";
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_CreateLanguagePartial", obj);
        }

        // GET: Language/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var LanguageFromDb = _unitOfWork.Language.Get(u => u.Id == id);
            if (LanguageFromDb == null)
            {
                return NotFound();
            }
            return PartialView("_EditLanguagePartial", LanguageFromDb);
        }

        // POST: Language/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Language obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Language.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Language updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_EditLanguagePartial", obj);
        }

        // GET: Language/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var LanguageFromDb = _unitOfWork.Language.Get(u => u.Id == id);
            if (LanguageFromDb == null)
            {
                return NotFound();
            }
            return PartialView("_DeleteLanguagePartial", LanguageFromDb);
        }

        // POST: Language/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obj = _unitOfWork.Language.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Language.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Language deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
