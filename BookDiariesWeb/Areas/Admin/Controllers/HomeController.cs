using BookDiaries.DataAccess.Repository.IRepository;
using BookDiaries.Models.Models;
using BookDiaries.Models.UserModels;
using BookDiaries.Models.ViewModels;
using BookDiaries.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookDiariesWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            AdminVM adminVM = new AdminVM
            {
                CategoryList = _unitOfWork.Category.GetAll(),
                LanguageList = _unitOfWork.Language.GetAll(),
                AuthorList = _unitOfWork.Author.GetAll(),
                ProductList = _unitOfWork.Product.GetAll(),
                OrderList = _unitOfWork.OrderHeader.GetAll(),
                UserList = _unitOfWork.AppUser.GetAll()
            };
            return View(adminVM);
        }
        public async Task<IActionResult> UserList()
        {
            var userList = await _userManager.Users.ToListAsync();
            var allUserList = userList.Select(x => new AllUser()
            {
                Id = x.Id,
                Username = x.UserName,
                Email = x.Email
            }).ToList();
            return View(allUserList);
        }
        public async Task<IActionResult> AssignRole(string Id)
        {
            var currentUser = (await _userManager.FindByIdAsync(Id))!;
            ViewBag.userId = Id;
            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var roleList = new List<AssignRole>();


            foreach (var role in roles)
            {
                var assignRole = new AssignRole() { Id = role.Id, Name = role.Name! };

                if (userRoles.Contains(role.Name!))
                {
                    assignRole.Exist = true;
                }

                roleList.Add(assignRole);

            }
            return View(roleList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, List<AssignRole> requestList)
        {
            var userToAssignRoles = (await _userManager.FindByIdAsync(userId))!;

            foreach (var role in requestList)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }
            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }
    }
}
