using EmployeesManagement.Data;
using EmployeesManagement.Models;
using EmployeesManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagement.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
            public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _signInManager = signInManager;
                _context = context;
            }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.Include(x=>x.Role).ToListAsync();
            return View(users);
        }
        public async Task<IActionResult> Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsersViewModel usersViewModel )
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", usersViewModel.RoleId);
            ApplicationUser user = new ApplicationUser();
            user.UserName = usersViewModel.UserName;
            user.FirstName = usersViewModel.FirstName;
            user.MiddleName = usersViewModel.MiddleName;
            user.LastName = usersViewModel.LastName;
            user.NationalId = usersViewModel.NationalId;
            
            user.NormalizedUserName = usersViewModel.UserName;
            user.Email = usersViewModel.Email;
            user.EmailConfirmed = true;
            user.PhoneNumber = usersViewModel.PhoneNumber;
            user.PhoneNumberConfirmed = true;
            user.CreatedOn = DateTime.Now;
            user.CreatedById = "Mutayyab Imran";
            user.RoleId = usersViewModel.RoleId;

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
