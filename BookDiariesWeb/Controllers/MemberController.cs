using BookDiaries.DataAccess.Data;
using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using BookDiaries.Models.UserModels;
using BookDiariesWeb.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;

namespace BookDiariesWeb.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileProvider _fileProvider;

        public MemberController(AppDbContext db, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider, IUnitOfWork unitOfWork)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
            _fileProvider = fileProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            //ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var editUser = new EditUser()
            {
                Username = currentUser.UserName!,
                Email = currentUser.Email!,
                PhoneNumber = currentUser.PhoneNumber!,
                FirstName = currentUser.FirstName!,
                LastName = currentUser.LastName!,
                BirthDate = currentUser.BirthDate!,
                City = currentUser.City,
                Gender = currentUser.Gender,
                Country = currentUser.Country,
                PostalCode = currentUser.PostalCode,
                StreetAddress = currentUser.StreetAddress,
                PictureUrl = currentUser.Picture
            };
            return View(editUser);

        }

        public IActionResult Orders()
        {
            // Retrieve the currently logged-in user
            var currentUser = _userManager.GetUserAsync(User).Result;

            // Check if the user exists
            if (currentUser == null)
            {
                // If the user is not found, you can handle it accordingly
                return NotFound();
            }

            // Retrieve orders associated with the current user
            IEnumerable<OrderHeader> userOrders = _unitOfWork.OrderHeader
                .GetAll(includeProperties: "AppUser")
                .Where(o => o.AppUserId == currentUser.Id)
                .ToList();

            return View(userOrders);
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, request.OldPassword);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Köhnə şifrəniz doğru deyil");
                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.OldPassword, request.NewPassword!);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors);
                return View();
            }


            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, request.NewPassword!, true, false);

            TempData["success"] = "Şifrəniz uğurla dəyişdirildi";



            return View();
        }


        public async Task<IActionResult> EditUser()
        {
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var editUserModel = new EditUser()
            {
                Username = currentUser.UserName!,
                Email = currentUser.Email!,
                PhoneNumber = currentUser.PhoneNumber!,
                FirstName = currentUser.FirstName!,
                LastName = currentUser.LastName!,
                BirthDate = currentUser.BirthDate!,
                City = currentUser.City,
                Country = currentUser.Country,
                PostalCode = currentUser.PostalCode,
                StreetAddress = currentUser.StreetAddress,
                Gender = currentUser.Gender,
                PictureUrl = currentUser.Picture
            };
            return View(editUserModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUser request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            currentUser.UserName = request.Username;
            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.PhoneNumber;
            currentUser.FirstName = request.FirstName;
            currentUser.LastName = request.LastName;
            currentUser.BirthDate = request.BirthDate;
            currentUser.City = request.City;
            currentUser.Country = request.Country;
            currentUser.StreetAddress = request.StreetAddress;
            currentUser.PostalCode = request.PostalCode;
            currentUser.Gender = request.Gender;




            if (request.Picture != null && request.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";

                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userPictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);

            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, true);


            TempData["success"] = "Istifadechi melumatlari ugurla deyishdirildi";

            var editUserModel = new EditUser()
            {
                Username = currentUser.UserName!,
                Email = currentUser.Email!,
                PhoneNumber = currentUser.PhoneNumber!,
                FirstName = currentUser.FirstName!,
                LastName = currentUser.LastName!,
                BirthDate = currentUser.BirthDate!,
                City = currentUser.City,
                Country = currentUser.Country,
                StreetAddress = currentUser.StreetAddress,
                PostalCode = currentUser.PostalCode,
                Gender = currentUser.Gender
            };
            return View(editUserModel);

        }

        public async Task<IActionResult> AccessDenied(string ReturnUrl)
        {
            return View();
        }
    }
}
