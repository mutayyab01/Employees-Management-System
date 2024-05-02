using EmployeesManagement.Data;
using EmployeesManagement.Models;
using EmployeesManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmployeesManagement.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProfilesController(ApplicationDbContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index()
        {
            var tasks = new ProfileViewModel();
            var roles = await _context.Roles.OrderBy(x => x.Name).ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            var systemtasks = await _context.SystemProfiles
                  .Include("Childern.Childern.Childern")
                  .OrderBy(x => x.Order)
                  .ToListAsync();
            ViewBag.Tasks = new SelectList(systemtasks, "Id", "Name");
            return View(tasks);
        }
        public async Task<IActionResult> AssignRights(ProfileViewModel vm)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var role = new RoleProfile()
            {
                TaskId = vm.TaskId,
                RoleId = vm.RoleId,
            };
            await _context.RoleProfiles.AddAsync(role);
            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }


    }
}
