using EmployeesManagement.Data;
using EmployeesManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagement.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
            public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _signInManager = signInManager;
                _context = context;
            }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }
        public async Task<IActionResult> Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsersViewModel usersViewModel )
        {
            IdentityUser user = new IdentityUser();
            user.UserName = usersViewModel.UserName;
            user.NormalizedUserName = usersViewModel.UserName;
            user.Email = usersViewModel.Email;
            user.EmailConfirmed = true;
            user.PhoneNumber = usersViewModel.PhoneNumber;
            user.PhoneNumberConfirmed = true;
          var result=  await _userManager.CreateAsync(user,usersViewModel.Password);
            if (result.Succeeded)
            {
            return RedirectToAction(nameof(Index));

            }
            else
            {
                return View(usersViewModel);
            }


        }

    }
}
