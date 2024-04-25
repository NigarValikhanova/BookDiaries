using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using BookDiaries.Models.UserModels;
using BookDiaries.Models.ViewModels;
using BookDiaries.Utility;
using BookDiariesWeb.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;
using System.Security.Claims;

namespace BookDiariesWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(
            ILogger<HomeController> logger, 
            IUnitOfWork unitOfWork, 
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            IEmailService emailService, 
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM
            {                
                SliderList = _unitOfWork.Slider.GetAll(),
                CategoryList = _unitOfWork.Category.GetAll(),
                LanguageList = _unitOfWork.Language.GetAll(),
                AuthorList = _unitOfWork.Author.GetAll(),
                ProductList = _unitOfWork.Product.GetAll(),
                BlogList = _unitOfWork.Blog.GetAll(),
                ProductImageList = _unitOfWork.ProductImage.GetAll(),
                ReviewList = _unitOfWork.RatingReview.GetAll()
            };
            return View(homeVM);
        }

        public IActionResult Coupons()
        {
            List<Coupon> objCouponList = _unitOfWork.Coupon.GetAll().ToList();
            return View(objCouponList);
        }

        public IActionResult Blogs()
        {
            List<Blog> blogList = _unitOfWork.Blog.GetAll().ToList();
            return View(blogList);
        }
        public IActionResult BlogDetails(int Id)
        {
            Blog blogDetails= _unitOfWork.Blog.Get(u => u.Id == Id);
            return View(blogDetails);
        }

        public IActionResult Register()
        {
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Seller)).GetAwaiter().GetResult();
            }
            var model = new Register();

            // Populate RoleList with available roles
            model.RoleList = _roleManager.Roles.Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Name
            });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = new AppUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Home", new { user.Id, token});

                //email
                await _emailService.SendEmailAsync(user.Email, "Email Confirmation", $"Please, click the link <a href='https://localhost:7175{url}'> below.</a>");

                if (User.IsInRole(SD.Role_Admin))
                {
                    TempData["success"] = "New User Created Successfully";
                }
                else
                {
                    TempData["success"] = "Click the confirm link in your mail, please";
                }

                if (!String.IsNullOrEmpty(model.Role))
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                }                              

                return RedirectToAction(nameof(HomeController.Register));
            }

            ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());

            return View(model);

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            returnUrl ??= Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(model.Email!);


            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email ve ya shifre duzgun deyil");
                return View();
            }

            if(!await _userManager.IsEmailConfirmedAsync(hasUser))
            {
                ModelState.AddModelError("", "Please, confirm your mail address, first");
                return View();            
            }

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password!, model.RememberMe, true);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl!);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "5 deqiqe boyunca girish ede bilmezsiniz" });
                return View();
            }

            ModelState.AddModelErrorList(new List<string>() { "Email ve ya shifre duzgun deyil", $"Ugursuz girish sayi={await _userManager.GetAccessFailedCountAsync(hasUser)}" });

            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPassword request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                TempData["error"] = "Email adresinizi daxil edin";
                return View();
            }
            var user = await _userManager.FindByEmailAsync(request.Email!);

            if (user == null)
            {
                ModelState.AddModelError(String.Empty, "Bu email adresine sahib istifadechi tapilmadi");
                return View();
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("ResetPassword", "Home", new { user.Id, token });


            await _emailService.SendEmailAsync(user.Email, "Şifrə yeniləmə linki", $"<h4>Shifrenizi yenilemek uchun asagidaki linke daxil olun</h4><p><a href='https://localhost:7175{url}'>Shifre yenileme link</a></p>");
            

            TempData["success"] = "Şifrə yeniləmə linki email adresinizə göndərilmişdir";

            return RedirectToAction(nameof(ForgetPassword));

        }

        public IActionResult ResetPassword(string Id, string token)
        {
            TempData["token"] = token;
            TempData["userId"] = Id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                throw new Exception("Sehv yarandi");
            }

            var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Istifadəçi tapılmadı");
            }

            // Check if the new password is the same as the old password
            var isSamePassword = await _userManager.CheckPasswordAsync(hasUser, request.Password);
            if (isSamePassword)
            {
                ModelState.AddModelError(String.Empty, "Yeni şifrə köhnə şifrə ilə eynidir. Zəhmət olmasa fərqli bir şifrə daxil edin.");
                return View();
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(hasUser!, token.ToString()!, request.Password!);

            if (result.Succeeded)
            {
                TempData["success"] = "Şifrəniz uğurla yeniləndi";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
            }

            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            if(Id == null || token == null)
            {
                TempData["error"] = "Invalid token";
                return View();
            }

            var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    TempData["success"] = "Your email is confirmed. Register completed";
                    return RedirectToAction("Login", "Home");
                }
            }
            TempData["error"] = "User has not found";
            return View();
        }


        //public IActionResult FacebookLogin(string ReturnUrl)
        //{
        //    string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });

        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", RedirectUrl);

        //    return new ChallengeResult("Facebook", properties);

        //}

        //public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")
        //{
        //    ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();

        //    if (info == null)
        //    {
        //        return RedirectToAction("Login");
        //    }
        //    else
        //    {
        //        Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

        //        if (result.Succeeded)
        //        {
        //            return Redirect(ReturnUrl);
        //        }
        //        else
        //        {
        //            AppUser user = new AppUser();

        //            user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
        //            string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

        //            if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
        //            {
        //                string userName = info.Principal.FindFirst(ClaimTypes.Name).Value;

        //                userName = userName.Replace(' ', '-').ToLower() + ExternalUserId.Substring(0, 5).ToString();

        //                user.UserName = userName;
        //            }
        //            else
        //            {
        //                user.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;
        //            }

        //            IdentityResult createResult = await _userManager.CreateAsync(user);
        //            if (createResult.Succeeded)
        //            {
        //                IdentityResult loginResult = await _userManager.AddLoginAsync(user, info);

        //                if (loginResult.Succeeded)
        //                {
        //                    ////await _signInManager.SignInAsync(user, true);

        //                    await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
        //                    return Redirect(ReturnUrl);
        //                }
        //                else
        //                {
        //                    ModelState.AddModelErrorList(loginResult.Errors.Select(x => x.Description).ToList());
        //                }
        //            }
        //            else
        //            {
        //                ModelState.AddModelErrorList(createResult.Errors.Select(x => x.Description).ToList());
        //            }
        //        }
        //    }

        //    return RedirectToAction("AccessDenied", "Member");
        //}

    }
}