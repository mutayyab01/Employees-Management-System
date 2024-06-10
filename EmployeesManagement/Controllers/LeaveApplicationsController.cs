using EmployeesManagement.Data;
using EmployeesManagement.Data.Migrations;
using EmployeesManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmployeesManagement.Controllers
{
    public class LeaveApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment webHostEnvironment;

        public LeaveApplicationsController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET: LeaveApplications
        public async Task<IActionResult> Index()
        {
            var AwaitingStatus = await _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveApprovalStatus" && y.Code == "AwaitingApproval").FirstOrDefaultAsync();

            var applicationDbContext = _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .Where(l => l.StatusId == AwaitingStatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }



        public async Task<IActionResult> ApproveLeaveApplications()
        {
            var ApproveStatus = await _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveApprovalStatus" && y.Code == "Approved").FirstOrDefaultAsync();

            var applicationDbContext = _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .Where(l => l.StatusId == ApproveStatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }


        public async Task<IActionResult> RejectedLeaveApplications()
        {
            var RejectedStatus = await _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveApprovalStatus" && y.Code == "Rejected").FirstOrDefaultAsync();

            var applicationDbContext = _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .Where(l => l.StatusId == RejectedStatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }



        // GET: LeaveApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        [HttpGet]
        public async Task<IActionResult> ApproveLeave(int? id)
        {
            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
            .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveApplication == null)
            {
                return NotFound();
            }
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode), "Id", "FullName");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");




            return View(leaveApplication);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLeave(LeaveApplication leave)
        {
            var ApproveStatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveApprovalStatus" && y.Code == "Approved")
                .FirstOrDefaultAsync();
            var AdjustmentType = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(y => y.SystemCode.Code == "LeaveAdjustment" && y.Code == "Negative")
                .FirstOrDefaultAsync();



            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
            .FirstOrDefaultAsync(m => m.Id == leave.Id);

            if (leaveApplication == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            leaveApplication.ApprovedOn = DateTime.Now;
            leaveApplication.ApprovedById = userId;
            leaveApplication.StatusId = ApproveStatus.Id;
            leaveApplication.ApprovalNotes = leave.ApprovalNotes;

            _context.LeaveApplications.Update(leaveApplication);
            await _context.SaveChangesAsync(userId);
            var adjustment = new LeaveAdjustmentEntry()
            {
                EmployeeId = leaveApplication.EmployeeId,
                NoOfDays = leaveApplication.NoOfDays,
                LeaveStartDate = leaveApplication.StartDate,
                LeaveEndDate = leaveApplication.EndDate,
                AdjustmentDescription = "Leave Taken Negative Adjusyment",
                LeavePeriodId = 1,
                LeaveAdjustmentDate = DateTime.Now,
                AdjustmentTypeId = AdjustmentType.Id,
            };

            _context.Add(adjustment);
            await _context.SaveChangesAsync(userId);

            var employee = await _context.Employees.FindAsync(leaveApplication.EmployeeId);
            employee.LeaveOutStandingBalance = (employee.AllocatedLeaveDays - leaveApplication.NoOfDays);

            _context.Update(employee);
            await _context.SaveChangesAsync(userId);





            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveDuration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> RejectLeave(int? id)
        {
            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
            .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveApplication == null)
            {
                return NotFound();
            }
            //ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode), "Id", "FullName");
            //ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            //ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");




            return View(leaveApplication);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectLeave(LeaveApplication leave)
        {
            var RejectStatus = await _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveApprovalStatus" && y.Code == "Rejected").FirstOrDefaultAsync();



            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
            .FirstOrDefaultAsync(m => m.Id == leave.Id);

            if (leaveApplication == null)
            {
                return NotFound();
            }
            leaveApplication.ApprovedOn = DateTime.Now;
            leaveApplication.ApprovedById = "Mutayyab Imran";
            leaveApplication.StatusId = RejectStatus.Id;
            leaveApplication.ApprovalNotes = leave.ApprovalNotes;

            _context.LeaveApplications.Update(leaveApplication);
            await _context.SaveChangesAsync();




            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveDuration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");




            return RedirectToAction(nameof(Index));



        }

        // GET: LeaveApplications/Create
        public IActionResult Create()
        {
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveDuration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
            return View();
        }

        // POST: LeaveApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveApplication leaveApplication, IFormFile leaveattachment)
        {
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveDuration"), "Id", "Description", leaveApplication.DurationId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveApplication.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveApplication.LeaveTypeId);
            if (leaveattachment != null)
            {
                var filename = "LeaveAttachment_" + DateTime.Now.ToString("dd-MM-yyyy hh mm ss tt") + "_" + leaveattachment.FileName;
                //var path = _configuration["FileSettings:UploadFolder"]!;
                string folderPath = Path.Combine(webHostEnvironment.WebRootPath, "LeaveAttachments");
                var filepath = Path.Combine(folderPath, filename);
                var steam = new FileStream(filepath, FileMode.Create);
                await leaveattachment.CopyToAsync(steam);
                leaveApplication.Attachment = filename;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pendingStatus = await _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.Code == "AwaitingApproval" && y.SystemCode.Code == "LeaveApprovalStatus").FirstOrDefaultAsync();
            leaveApplication.CreatedOn = DateTime.Now;
            leaveApplication.CreatedById = "Mutayyab Imran";
            leaveApplication.StatusId = pendingStatus.Id;

            _context.Add(leaveApplication);
            await _context.SaveChangesAsync(userId);


            //Leave Type
            var documenttype = await _context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "DocumentTypes" && x.Code == "LeaveApplication").FirstOrDefaultAsync();

            //Workflow User Group
            var usergroup = await _context.ApprovalUserMatrixs.Where(x => x.UserId == userId && x.DocumentTypeId == documenttype.Id && x.Active == true).FirstOrDefaultAsync();


            var awaitingapproval = await _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.Code == "AwaitingApproval" && y.SystemCode.Code == "LeaveApprovalStatus").FirstOrDefaultAsync();


            // Approvers
            var approvers = await _context.WorkFlowUserGroupMembers.Where(x => x.WorkFlowUserGroupId == usergroup.WorkflowUserGroupId && x.SenderId == userId).ToListAsync();

            foreach (var approver in approvers)
            {
                //Generate an approval Entry
                var approvalentries = new ApprovalEntry()
                {
                    ApproverId = approver.ApproverId,
                    DateSentForApproval = DateTime.Now,
                    LastModifiedOn = DateTime.Now,
                    LastModifiedById = approver.SenderId,
                    RecordId = leaveApplication.Id,
                    ControllerName = "LeaveApplications",
                    DocumentTypeId = documenttype.Id,
                    SequenceNo = approver.SequenceNo,
                    StatusId = awaitingapproval.Id,
                    Comments = "Sent For Approvals",
                };


                _context.Add(approvalentries);
                await _context.SaveChangesAsync(userId);
            }

            return RedirectToAction(nameof(Index));

            return View(leaveApplication);
        }

        // GET: LeaveApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications.FindAsync(id);
            if (leaveApplication == null)
            {
                return NotFound();
            }
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.SystemCode.Code == "LeaveDuration"), "Id", "Description", leaveApplication.DurationId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveApplication.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveApplication.LeaveTypeId);
            return View(leaveApplication);
        }

        // POST: LeaveApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveApplication leaveApplication)
        {
            if (id != leaveApplication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var pendingStatus = _context.SystemCodeDetails.Include(x => x.SystemCode).Where(y => y.Code == "Pending" && y.SystemCode.Code == "LeaveApprovalStatus").FirstOrDefault();

                try
                {
                    leaveApplication.ModifiedOn = DateTime.Now;
                    leaveApplication.ModifiedById = "Mutayyab Imran";
                    leaveApplication.StatusId = pendingStatus.Id;

                    _context.Update(leaveApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveApplicationExists(leaveApplication.Id))
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
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", leaveApplication.DurationId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveApplication.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveApplication.LeaveTypeId);
            return View(leaveApplication);
        }

        // GET: LeaveApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        // POST: LeaveApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveApplication = await _context.LeaveApplications.FindAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (leaveApplication != null)
            {
                _context.LeaveApplications.Remove(leaveApplication);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveApplicationExists(int id)
        {
            return _context.LeaveApplications.Any(e => e.Id == id);
        }
    }
}
