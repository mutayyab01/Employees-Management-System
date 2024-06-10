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
    public class ApprovalUserMatricesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApprovalUserMatricesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ApprovalUserMatrices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ApprovalUserMatrixs.Include(a => a.DocumentType).Include(a => a.User).Include(a => a.WorkflowUserGroup);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ApprovalUserMatrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalUserMatrix = await _context.ApprovalUserMatrixs
                .Include(a => a.DocumentType)
                .Include(a => a.User)
                .Include(a => a.WorkflowUserGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approvalUserMatrix == null)
            {
                return NotFound();
            }

            return View(approvalUserMatrix);
        }

        // GET: ApprovalUserMatrices/Create
        public IActionResult Create()
        {
            ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "DocumentTypes"), "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["WorkflowUserGroupId"] = new SelectList(_context.WorkFlowUserGroups, "Id", "Description");
            return View();
        }

        // POST: ApprovalUserMatrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApprovalUserMatrix approvalUserMatrix)
        {
            ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "DocumentTypes"), "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", approvalUserMatrix.UserId);
            ViewData["WorkflowUserGroupId"] = new SelectList(_context.WorkFlowUserGroups, "Id", "Description", approvalUserMatrix.WorkflowUserGroupId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            approvalUserMatrix.CreatedById = userId;
            approvalUserMatrix.CreatedOn = DateTime.Now;

            _context.Add(approvalUserMatrix);
            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));


            return View(approvalUserMatrix);
        }

        // GET: ApprovalUserMatrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalUserMatrix = await _context.ApprovalUserMatrixs.FindAsync(id);
            if (approvalUserMatrix == null)
            {
                return NotFound();
            }
            ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "DocumentTypes"), "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", approvalUserMatrix.UserId);
            ViewData["WorkflowUserGroupId"] = new SelectList(_context.WorkFlowUserGroups, "Id", "Description", approvalUserMatrix.WorkflowUserGroupId);
            return View(approvalUserMatrix);
        }

        // POST: ApprovalUserMatrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApprovalUserMatrix approvalUserMatrix)
        {
            if (id != approvalUserMatrix.Id)
            {
                return NotFound();
            }


            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                approvalUserMatrix.ModifiedById = userId;
                approvalUserMatrix.ModifiedOn = DateTime.Now;
                _context.Update(approvalUserMatrix);
                await _context.SaveChangesAsync(userId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApprovalUserMatrixExists(approvalUserMatrix.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "DocumentTypes"), "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", approvalUserMatrix.UserId);
            ViewData["WorkflowUserGroupId"] = new SelectList(_context.WorkFlowUserGroups, "Id", "Description", approvalUserMatrix.WorkflowUserGroupId);
            return View(approvalUserMatrix);
        }

        // GET: ApprovalUserMatrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalUserMatrix = await _context.ApprovalUserMatrixs
                .Include(a => a.DocumentType)
                .Include(a => a.User)
                .Include(a => a.WorkflowUserGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approvalUserMatrix == null)
            {
                return NotFound();
            }

            return View(approvalUserMatrix);
        }

        // POST: ApprovalUserMatrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var approvalUserMatrix = await _context.ApprovalUserMatrixs.FindAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (approvalUserMatrix != null)
            {
                _context.ApprovalUserMatrixs.Remove(approvalUserMatrix);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool ApprovalUserMatrixExists(int id)
        {
            return _context.ApprovalUserMatrixs.Any(e => e.Id == id);
        }
    }
}
