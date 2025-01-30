using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet.Data;
using Projet.Models;

namespace Projet.Controllers
{
    public class InterviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InterviewsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Interviews
        public async Task<IActionResult> Index()
        {
            var interviews = await _context.Interviews
                .Include(i => i.JobApplication)
                .ToListAsync();

            return View(interviews);
        }


        // GET: Interviews/ChangeDate/5
        public IActionResult ChangeDate(int id)
        {
            var interview = _context.Interviews.FirstOrDefault(i => i.InterviewId == id);

            if (interview == null || interview.Status != "Scheduled")
            {
                return NotFound();
            }

            var viewModel = new ChangeInterviewDateViewModel
            {
                InterviewId = interview.InterviewId,
                NewInterviewDate = interview.ScheduledDate
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ChangeDate(ChangeInterviewDateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var interview = _context.Interviews.FirstOrDefault(i => i.InterviewId == model.InterviewId);
            if (interview == null || interview.Status != "Scheduled")
            {
                return NotFound();
            }

            interview.ScheduledDate = model.NewInterviewDate;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Interviews/GiveFeedback/5
        public IActionResult GiveFeedback(int id)
        {
            var interview = _context.Interviews.Include(i => i.JobApplication).FirstOrDefault(i => i.InterviewId == id);
            if (interview == null || interview.Status != "Scheduled")
            {
                return NotFound();
            }

            var viewModel = new GiveFeedbackViewModel
            {
                InterviewId = interview.InterviewId,
                Feedback = interview.Feedback
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult GiveFeedback(GiveFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var interview = _context.Interviews.Include(i => i.JobApplication).FirstOrDefault(i => i.InterviewId == model.InterviewId);
            if (interview == null || interview.Status != "Scheduled")
            {
                return NotFound();
            }

            interview.Feedback = model.Feedback;
            interview.Status = "Need Decision"; // Change job application status
            interview.JobApplication.Status = "Need Decision";

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Cancel(int id)
        {
            var interview = _context.Interviews.FirstOrDefault(i => i.InterviewId == id);
            if (interview == null || interview.Status != "Scheduled")
            {
                return NotFound();
            }

            interview.Status = "Cancelled";
            _context.SaveChanges();

            // Return the same view (no redirect)
            return View("Index", _context.Interviews.ToList());
        }

    }
}
