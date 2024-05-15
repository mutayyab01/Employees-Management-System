using EmployeesManagement.Data;
using EmployeesManagement.Data.Migrations;
using EmployeesManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmployeesManagement.Controllers
{
    public class LeaveBalancesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LeaveBalancesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var results = await _context.Employees
                .Include(x => x.Status)
                .ToListAsync();
            return View(results);

        }
        public IActionResult AdjustLeaveBalance(int id)
        {
            LeaveAdjustmentEntry leaveadjustment = new LeaveAdjustmentEntry();
            leaveadjustment.EmployeeId = id;

            ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(y => y.SystemCode).Where(x => x.SystemCode.Code == "LeaveAdjustment"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", id);
            return View(leaveadjustment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustLeaveBalance(LeaveAdjustmentEntry leaveAdjustmentEntry)
        {
            ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", leaveAdjustmentEntry.AdjustmentTypeId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveAdjustmentEntry.EmployeeId);
          
            var adjustmenttype = await _context.SystemCodeDetails
             .Include(x => x.SystemCode)
             .Where(y => y.SystemCode.Code == "LeaveAdjustment" && y.Id == leaveAdjustmentEntry.AdjustmentTypeId)
             .FirstOrDefaultAsync();

            leaveAdjustmentEntry.AdjustmentDescription = leaveAdjustmentEntry.AdjustmentDescription + "-" + adjustmenttype.Description;
            leaveAdjustmentEntry.Id = 0;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Add(leaveAdjustmentEntry);
            await _context.SaveChangesAsync(userId);

            var employee = await _context.Employees.FindAsync(leaveAdjustmentEntry.EmployeeId);
            if (adjustmenttype.Code == "Positive")
            {
                employee.LeaveOutStandingBalance = (employee.AllocatedLeaveDays + leaveAdjustmentEntry.NoOfDays);
            }
            else
            {
                employee.LeaveOutStandingBalance = (employee.AllocatedLeaveDays - leaveAdjustmentEntry.NoOfDays);
            }
            _context.Update(employee);
            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));


            return View(leaveAdjustmentEntry);
        }
    }
}
