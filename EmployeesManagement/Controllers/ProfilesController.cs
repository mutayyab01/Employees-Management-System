using EmployeesManagement.Data;
using EmployeesManagement.Models;
using EmployeesManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
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
        public async Task<IActionResult> UserRights(string id)
        {
            var tasks = new ProfileViewModel();
            tasks.RoleId=id;
            tasks.profiles = await _context.SystemProfiles
                .Include(s=>s.Profile)
                 .Include("Childern.Childern.Childern")
                 .OrderBy(x => x.Order)
                 .ToListAsync();
            tasks.RoleRightsIds = await _context.RoleProfiles.Where(x => x.RoleId == id).Select(r => r.TaskId).ToListAsync();


            return View(tasks);
        }
        [HttpPost]
        public async Task<ActionResult> UserGroupRights(string id, ProfileViewModel vm)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allright=await  _context.RoleProfiles.Where(x=>x.RoleId==id).ToListAsync();
            _context.RoleProfiles.RemoveRange(allright);
            await _context.SaveChangesAsync(UserId);
            foreach (var taskId in vm.Ids.Distinct())
            {
                var role = new RoleProfile
                {
                    TaskId = taskId,
                    RoleId = id
                };

                _context.RoleProfiles.Add(role);
                await _context.SaveChangesAsync(UserId);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
