using EmployeesManagement.Data;
using EmployeesManagement.Models;
using EmployeesManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmployeesManagement.Controllers
{
    public class RolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;


        public RolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var role = await _context.Roles.ToListAsync();
            return View(role);
        }
        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RolesViewModel model)
        {
            IdentityRole role = new IdentityRole();
            role.Name = model.RoleName;
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));

            }
            else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = new RolesViewModel();
            var result = await _roleManager.FindByIdAsync(id);
            role.RoleName = result.Name;
            role.Id = result.Id;

            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, RolesViewModel model)
        {
            var checkifexist = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!checkifexist)
            {

                var result = await _roleManager.FindByIdAsync(id);
                result.Name = model.RoleName;

                var finalresult = await _roleManager.UpdateAsync(result);
                if (finalresult.Succeeded)
                {
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    return View(model);
                }
            }
            return View(model);

        }
    }
}
