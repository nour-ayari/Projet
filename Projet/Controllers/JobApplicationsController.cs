using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Projet.Models;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Projet.Data;

namespace Projet.Controllers
{
    public class JobApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public JobApplicationsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: JobApplications
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.JobApplications.Include(j => j.Job);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: JobApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications
                .Include(j => j.Job)
                .FirstOrDefaultAsync(m => m.JobApplicationId == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // GET: JobApplication/Create
        public IActionResult Create()
        {
            ViewData["JobId"] = new SelectList(_context.Jobs, "JobId", "JobId");
            return View();
        }

        // Action for scheduling an interview
        public IActionResult Schedule(int id)
        {
            var jobApplication = _context.JobApplications
                .Include(j => j.Job)
                .FirstOrDefault(j => j.JobApplicationId == id);

            if (jobApplication == null)
            {
                return NotFound();  
            }
            return View(jobApplication);  
        }

        // POST: JobApplication/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobApplication jobApplication, IFormFile resume)
        {
            if (ModelState.IsValid)
            {
                if (resume != null && resume.Length > 0)
                {
                    // Create the uploads folder if it doesn't exist
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    // Generate a unique file name and save the file
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(resume.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await resume.CopyToAsync(fileStream);
                    }

                    jobApplication.Resume = "/uploads/" + uniqueFileName;
                }

                _context.Add(jobApplication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["JobId"] = new SelectList(_context.Jobs, "JobId", "Title", jobApplication.JobId);
            return View(jobApplication);
        }

        [HttpPost]
        public IActionResult ScheduleInterview(Interview interview)
        {
            if (ModelState.IsValid)
            {
                var jobApplication = _context.JobApplications
                    .FirstOrDefault(j => j.JobApplicationId == interview.JobApplicationId);

                if (jobApplication != null)
                {
                    _context.Interviews.Add(interview);
                    _context.SaveChanges();

                    jobApplication.Status = "Interview Scheduled";
                    _context.SaveChanges();

                    return RedirectToAction("Details", new { id = interview.JobApplicationId }); // Redirect to details page of the job application
                }
            }

            return View(interview);          }


        // GET: JobApplication/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            ViewData["JobId"] = new SelectList(_context.Jobs, "JobId", "JobId", jobApplication.JobId);
            return View(jobApplication);
        }

        // POST: JobApplication/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobApplication jobApplication, IFormFile resume)
        {
            if (id != jobApplication.JobApplicationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (resume != null && resume.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(resume.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await resume.CopyToAsync(fileStream);
                        }

                        // Delete the old file if exists
                        if (!string.IsNullOrEmpty(jobApplication.Resume))
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, jobApplication.Resume.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        jobApplication.Resume = "/uploads/" + uniqueFileName;
                    }

                    _context.Update(jobApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobApplicationExists(jobApplication.JobApplicationId))
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

            ViewData["JobId"] = new SelectList(_context.Jobs, "JobId", "JobId", jobApplication.JobId);
            return View(jobApplication);
        }

        // GET: JobApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications
                .Include(j => j.Job)
                .FirstOrDefaultAsync(m => m.JobApplicationId == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // POST: JobApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication != null)
            {
                if (!string.IsNullOrEmpty(jobApplication.Resume))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, jobApplication.Resume.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.JobApplications.Remove(jobApplication);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool JobApplicationExists(int id)
        {
            return _context.JobApplications.Any(e => e.JobApplicationId == id);
        }
    }
}
