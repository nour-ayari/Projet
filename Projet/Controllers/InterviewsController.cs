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

            // Update the interview status to "Cancelled"

            interview.Status = "Cancelled";
            var jobApplication = interview.JobApplication;
            jobApplication.Status = "Interview Cancelled";

            _context.SaveChanges();

            // Redirect to the Index action to refresh the page
            return RedirectToAction("Index");
        }
        public IActionResult InterviewDetails(int id)
        {
            var interview = _context.Interviews
                .Include(i => i.JobApplication)
                .ThenInclude(ja => ja.Job)
                .FirstOrDefault(i => i.InterviewId== id);

            if (interview == null)
            {
                return NotFound(); 
            }

            return View(interview); 
        }
        [HttpPost]
        public IActionResult MarkAsInterviewed(int id)
        {
            var interview = _context.Interviews.FirstOrDefault(i => i.InterviewId == id);
            if (interview == null || interview.Status != "Scheduled")
            {
                return NotFound();
            }

            // Update the interview status to "Interviewed"
            interview.Status = "Interviewed";
            var jobApplication = interview.JobApplication;
            jobApplication.Status = "Interviewed";
            _context.SaveChanges();

            // Redirect to the Index page
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Accept(int id)
        {
            var interview = _context.Interviews.FirstOrDefault(i => i.InterviewId == id);
            if (interview == null || interview.Status != "Need Decision")
            {
                return NotFound();
            }

            // Update the interview status to "Accepted"
            interview.Status = "Accepted";
            var jobApplication = interview.JobApplication;
            jobApplication.Status = "Accepted";
            _context.SaveChanges();

            // Redirect to the Index page
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Reject(int id)
        {
            var interview = _context.Interviews.FirstOrDefault(i => i.InterviewId == id);
            if (interview == null || interview.Status != "Need Decision")
            {
                return NotFound();
            }

            // Update the interview status to "Rejected"
            interview.Status = "Rejected";
            var jobApplication = interview.JobApplication;
            jobApplication.Status = "Rejected";
            _context.SaveChanges();

            // Redirect to the Index page
            return RedirectToAction("Index");
        }
    }

}    
       
