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
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;

namespace ASM1670_Job.Controllers
{
    public class SeekersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public SeekersController(ApplicationDbContext context,IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        [Authorize(Roles ="Seeker")]
        // GET: Seekers
        public async Task<IActionResult> Index()
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Lấy danh sách công việc của nhà tuyển dụng hiện tại
            var seecker = await _context.Seeker
                .Where(j => j.SeekerId == currentUserId)
                .Include(a => a.User_ID) // Bao gồm thông tin của Seeker
                .ToListAsync();

            return View(seecker);
        }

        // GET: Seekers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Seeker == null)
            {
                return NotFound();
            }

            var seeker = await _context.Seeker
                .Include(s => s.User_ID)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seeker == null)
            {
                return NotFound();
            }

            return View(seeker);
        }

        // GET: Seekers/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["SeekerId"] = new SelectList(_context.Users.Where(u => u.Id == userId), "Id", "Email");
            return View();
           
        }

        // POST: Seekers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SeekerId,Name,Age,Gender,PictureImage")] Seeker seeker)
        {
            if (ModelState.IsValid)
            {
                if (seeker.PictureImage != null)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + seeker.PictureImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await seeker.PictureImage.CopyToAsync(fileStream);
                    }
                    seeker.Picture = "/images/" + uniqueFileName;
                }
                _context.Add(seeker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeekerId"] = new SelectList(_context.Users, "Id", "Email", seeker.SeekerId);
            return View(seeker);
        }

        // GET: Seekers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Seeker == null)
            {
                return NotFound();
            }

            var seeker = await _context.Seeker.FindAsync(id);
            if (seeker == null)
            {
                return NotFound();
            }
            ViewData["SeekerId"] = new SelectList(_context.Users, "Id", "Email", seeker.SeekerId);
            return View(seeker);
        }

        // POST: Seekers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,SeekerId,Name,Age,Gender,PictureImage")] Seeker seeker)
        {
            if (id == null || id != seeker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var seekerToUpdate = await _context.Seeker.FindAsync(id);
                    if (seekerToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các thuộc tính của seeker
                    seekerToUpdate.Name = seeker.Name;
                    seekerToUpdate.Age = seeker.Age;
                    seekerToUpdate.Gender = seeker.Gender;

                    // Cập nhật ảnh nếu có
                    if (seeker.PictureImage != null)
                    {
                        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + seeker.PictureImage.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await seeker.PictureImage.CopyToAsync(fileStream);
                        }
                        seekerToUpdate.Picture = "/images/" + uniqueFileName;
                    }

                    _context.Update(seekerToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeekerExists(seeker.Id))
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
            ViewData["SeekerId"] = new SelectList(_context.Users, "Id", "Email", seeker.SeekerId);
            return View(seeker);
        }
        // GET: Seekers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Seeker == null)
            {
                return NotFound();
            }

            var seeker = await _context.Seeker
                .Include(s => s.User_ID)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seeker == null)
            {
                return NotFound();
            }

            return View(seeker);
        }

        // POST: Seekers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Seeker == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Seeker'  is null.");
            }
            var seeker = await _context.Seeker.FindAsync(id);
            if (seeker != null)
            {
                _context.Seeker.Remove(seeker);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeekerExists(int id)
        {
          return (_context.Seeker?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
