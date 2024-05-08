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
using Microsoft.AspNetCore.Hosting;

namespace EmployeesManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment webHostEnvironment;
        public EmployeesController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.Include(x => x.Status).ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(x=>x.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name");
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "EmploymentTerms"), "Id", "Description");
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Gender"), "Id", "Description");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee, IFormFile employeephoto)
        {
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", employee.BankId);
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "EmploymentTerms"), "Id", "Description", employee.EmploymentTermsId);
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Gender"), "Id", "Description", employee.GenderId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", employee.CountryId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DesignationId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            if (employeephoto!=null)
            {
                var ext = Path.GetExtension(employeephoto.FileName);
                var size = employeephoto.Length;
                if (ext == ".png" || ext == ".jpeg" || ext == ".jpg")
                {
                    if (size <= 1000000)//1Mb
                    {
                        var filename = "EmployeePhoto_" + DateTime.Now.ToString("dd-MM-yyyy hh mm ss tt") + "_" + employeephoto.FileName;
                        //var path = _configuration["FileSettings:UploadFolder"]!;
                        string folderPath = Path.Combine(webHostEnvironment.WebRootPath, "EmployeePictures");
                        var filepath = Path.Combine(folderPath, filename);
                        var steam = new FileStream(filepath, FileMode.Create);
                        await employeephoto.CopyToAsync(steam);
                        employee.Photo = filename;
                    }
                    else
                    {
                        TempData["sizeError"] = "Image Must be less Than 1 Mb";
                        return View(employee);
                    }
                }
                else
                {
                    TempData["extError"] = "Only PNG,JPEG,JPG Pictures Are Allowed";
                    return View(employee);

                }
            }

            var StatusId = await _context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "EmployeeStatus" && x.Code == "Active").FirstOrDefaultAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            employee.CreatedById = userId;
            employee.CreatedOn = DateTime.Now;
            employee.StatusId = StatusId.Id;


            _context.Add(employee);
            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
           
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Gender"), "Id", "Description");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmpNo,FirstName,MiddleName,LastName,PhoneNumber,EmailAddress,Country,DateOfBirth,Address,Department,Designation,CreatedById,CreatedOn,ModifiedById,ModifiedOn")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Gender"), "Id", "Description", employee.GenderId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", employee.CountryId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DesignationId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
