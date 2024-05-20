﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesManagement.Data;
using EmployeesManagement.Models;
using System.Security.Claims;
using EmployeesManagement.Data.Migrations;
using Microsoft.AspNetCore.Hosting;
using CompanyInformation = EmployeesManagement.Models.CompanyInformation;

namespace EmployeesManagement.Controllers
{
    public class CompanyInformationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyInformationsController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: CompanyInformations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CompanyInformations.Include(c => c.City).Include(c => c.Country);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CompanyInformations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyInformation = await _context.CompanyInformations
                .Include(c => c.City)
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyInformation == null)
            {
                return NotFound();
            }

            return View(companyInformation);
        }

        // GET: CompanyInformations/Create
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            return View();
        }

        // POST: CompanyInformations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyInformation companyInformation,IFormFile logo)
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", companyInformation.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", companyInformation.CountryId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (logo != null)
            {
                var ext = Path.GetExtension(logo.FileName);
                var size = logo.Length;
                if (ext == ".png" || ext == ".jpeg" || ext == ".jpg")
                {
                    if (size <= 1000000)//1Mb
                    {
                        var filename = "CompanyLogo_" + DateTime.Now.ToString("dd-MM-yyyy hh mm ss tt") + "_" + logo.FileName;
                        //var path = _configuration["FileSettings:UploadFolder"]!;
                        string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "CompanyLogo");
                        var filepath = Path.Combine(folderPath, filename);
                        var steam = new FileStream(filepath, FileMode.Create);
                        await logo.CopyToAsync(steam);
                        companyInformation.Logo = filename;
                    }
                    else
                    {
                        TempData["sizeError"] = "Image Must be less Than 1 Mb";
                        return View(companyInformation);
                    }
                }
                else
                {
                    TempData["extError"] = "Only PNG,JPEG,JPG Pictures Are Allowed";
                    return View(companyInformation);

                }
            }








            _context.Add(companyInformation);
            await _context.SaveChangesAsync(userId);

            return RedirectToAction(nameof(Index));

            return View(companyInformation);
        }

        // GET: CompanyInformations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyInformation = await _context.CompanyInformations.FindAsync(id);
            if (companyInformation == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", companyInformation.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", companyInformation.CountryId);
            return View(companyInformation);
        }

        // POST: CompanyInformations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,PhoneNo,NSSFNO,NHIFNO,KRAPIN,ContactPerson,Logo,PostalCode,CityId,CountryId")] CompanyInformation companyInformation)
        {
            if (id != companyInformation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyInformation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyInformationExists(companyInformation.Id))
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
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", companyInformation.CityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", companyInformation.CountryId);
            return View(companyInformation);
        }

        // GET: CompanyInformations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyInformation = await _context.CompanyInformations
                .Include(c => c.City)
                .Include(c => c.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyInformation == null)
            {
                return NotFound();
            }

            return View(companyInformation);
        }

        // POST: CompanyInformations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var companyInformation = await _context.CompanyInformations.FindAsync(id);
            if (companyInformation != null)
            {
                _context.CompanyInformations.Remove(companyInformation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyInformationExists(int id)
        {
            return _context.CompanyInformations.Any(e => e.Id == id);
        }
    }
}
