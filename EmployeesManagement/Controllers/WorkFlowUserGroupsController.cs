using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesManagement.Data;
using EmployeesManagement.Models;
using System.Security.Claims;

namespace EmployeesManagement.Controllers
{
    public class WorkFlowUserGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkFlowUserGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkFlowUserGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkFlowUserGroups.ToListAsync());
        }

        // GET: WorkFlowUserGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workFlowUserGroup = await _context.WorkFlowUserGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workFlowUserGroup == null)
            {
                return NotFound();
            }

            return View(workFlowUserGroup);
        }

        // GET: WorkFlowUserGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkFlowUserGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Description")] WorkFlowUserGroup workFlowUserGroup)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(workFlowUserGroup);
                await _context.SaveChangesAsync(userId);
                return RedirectToAction(nameof(Index));
            }
            return View(workFlowUserGroup);
        }

        // GET: WorkFlowUserGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workFlowUserGroup = await _context.WorkFlowUserGroups.FindAsync(id);
            if (workFlowUserGroup == null)
            {
                return NotFound();
            }
            return View(workFlowUserGroup);
        }

        // POST: WorkFlowUserGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Description")] WorkFlowUserGroup workFlowUserGroup)
        {
            if (id != workFlowUserGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _context.Update(workFlowUserGroup);
                    await _context.SaveChangesAsync(userId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkFlowUserGroupExists(workFlowUserGroup.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workFlowUserGroup);
        }

        // GET: WorkFlowUserGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workFlowUserGroup = await _context.WorkFlowUserGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workFlowUserGroup == null)
            {
                return NotFound();
            }

            return View(workFlowUserGroup);
        }

        // POST: WorkFlowUserGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workFlowUserGroup = await _context.WorkFlowUserGroups.FindAsync(id);
            if (workFlowUserGroup != null)
            {
                _context.WorkFlowUserGroups.Remove(workFlowUserGroup);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkFlowUserGroupExists(int id)
        {
            return _context.WorkFlowUserGroups.Any(e => e.Id == id);
        }
    }
}
