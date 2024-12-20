using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASM1670_Job.Data;
using ASM1670_Job.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using NuGet.Configuration;

namespace ASM1670_Job.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles ="Seeker")]
        // GET: Applications
        public async Task<IActionResult> Index()
        {
            // Lấy ID của nhà tuyển dụng hiện tại
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Lấy danh sách công việc của nhà tuyển dụng hiện tại
            var applications = await _context.Application
                .Where(j => j.Seeker.SeekerId == currentUserId)
                .Include(a => a.Seeker) // Bao gồm thông tin của Seeker
                 .Include(a => a.Job) // Bao gồm thông tin của Job
                /*                            .Wher*/
                .ToListAsync();

            return View(applications);
        }


        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Application == null)
            {
                return NotFound();
            }

            var application = await _context.Application
                .Include(a => a.Job)
                .Include(a => a.Seeker)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        [Authorize(Roles = "Seeker")]
        // GET: Applications/Create
        public IActionResult Create(int? jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewData["Seeker_Id"] = new SelectList(_context.Seeker.Where(u => u.SeekerId == userId), "Id", "Name");
            //ViewData["Seeker_Id"] = new SelectList(_context.Seeker, "Id", "Name");
            //ViewData["Job_Id"] = new SelectList(_context.Job, "Id", "Name");
            ViewData["Job_Id"] = jobId.HasValue ? new SelectList(_context.Job.Where(j => j.Id == jobId), "Id", "Title", jobId) : new SelectList(_context.Job, "Id", "Title");
            return View();

        }

        // POST: Applications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        //Employer
        public async Task<IActionResult> Create([Bind("Id,ApplicationLatter,ApplicationDate,Seeker_Id,Job_Id")] Application application)
        {
            if (ModelState.IsValid)
            {
                //Gán mặc định status là Not Comfirmed.
                application.Status = "Not Confirmed";
                _context.Add(application);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Job_Id"] = new SelectList(_context.Job, "Id", "Title", application.Job_Id);
            ViewData["Seeker_Id"] = new SelectList(_context.Seeker, "Id", "Name", application.Seeker_Id);
            return View(application);
        }
        
        public async Task<IActionResult> ConfirmApplication(int id)
        {
            
            var application = await _context.Application.FindAsync(id); //lay application theo Id trong database
            if (application == null)
            {
                return NotFound();
            }

            // Update status to "Confirmed"
            application.Status = "Confirmed"; //gán lại status là confirmed
            _context.Application.Update(application); //update lại application trong bảng Model application
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EmployerApplication));
        }
        
        [HttpPost]
        
        public async Task<IActionResult> RefuseApplication(int id)
        {
            var application = await _context.Application.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            // Update status to "Refuse"
            application.Status = "Refuse";
            _context.Application.Update(application);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EmployerApplication));
        }
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> EmployerApplication()
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);//lay id cua user dang dang nhap


            var applications = await _context.Application
                .Where(a => a.Job.Employer.EmployerId == currentUserId) //lọc những application theo id của user đang đăng nhập
                .Include(a => a.Seeker) 
                .Include(a => a.Job)
                .ToListAsync();

            return View("EmployerApplication", applications);

        }

        [Authorize(Roles = "Seeker")]
        // GET: Applications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Application == null)
            {
                return NotFound();
            }

            var application = await _context.Application.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            ViewData["Job_Id"] = new SelectList(_context.Job, "Id", "Title", application.Job_Id);
            ViewData["Seeker_Id"] = new SelectList(_context.Seeker, "Id", "Name", application.Seeker_Id);
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationLatter,ApplicationDate,Seeker_Id,Job_Id")] Application application)
        {
            if (id != application.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(application);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(application.Id))
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
            ViewData["Job_Id"] = new SelectList(_context.Job, "Id", "Title", application.Job_Id);
            ViewData["Seeker_Id"] = new SelectList(_context.Seeker, "Id", "Name", application.Seeker_Id);
            return View(application);
        }

        // GET: Applications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Application == null)
            {
                return NotFound();
            }

            var application = await _context.Application
                .Include(a => a.Job)
                .Include(a => a.Seeker)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Application == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Application'  is null.");
            }
            var application = await _context.Application.FindAsync(id);
            if (application != null)
            {
                _context.Application.Remove(application);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationExists(int id)
        {
          return (_context.Application?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
