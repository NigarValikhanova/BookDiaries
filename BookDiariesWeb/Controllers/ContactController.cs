using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookDiariesWeb.Controllers
{
    public class ContactController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ContactUser obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ContactUser.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Your message sent successfully";
                return RedirectToAction("Index");
            }
            return View();

        }
    }
}
