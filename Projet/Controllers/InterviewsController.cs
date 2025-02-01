using System;
using System.Linq;
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
        public IActionResult Index(string filter)
        {
            var interviews = _context.Interviews
                .Include(i => i.JobApplication)
                .AsQueryable();

            switch (filter)
            {
                case "feedback":
                    interviews = interviews.Where(i => i.Status == "Interviewed");
                    break;
                case "decision":
                    interviews = interviews.Where(i => i.Status == "Need Decision");
                    break;
                default:
                    break;
            }

            return View(interviews.ToList());
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
            interview.Status = "Need Decision"; // Change interview status
            interview.JobApplication.Status = "Need Decision"; // Update job application status

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // POST: Interviews/Cancel/5
        public IActionResult Cancel(int id)
        {
            var interview = _context.Interviews.Include(i => i.JobApplication).FirstOrDefault(i => i.InterviewId == id);
            if (interview == null || interview.Status != "Scheduled")
            {
                return NotFound();
            }

            // Update the interview status to "Cancelled"
            interview.Status = "Cancelled";
            interview.JobApplication.Status = "Cancelled"; // Update job application status

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Interviews/InterviewDetails/5
        public IActionResult InterviewDetails(int id)
        {
            var interview = _context.Interviews
                .Include(i => i.JobApplication)
                .ThenInclude(ja => ja.Job)
                .FirstOrDefault(i => i.InterviewId == id);

            if (interview == null)
            {
                return NotFound();
            }

            return View(interview);
        }

        // POST: Interviews/MarkAsInterviewed/5
        [HttpPost]
        public IActionResult MarkAsInterviewed(int id)
        {
            var interview = _context.Interviews.Include(i => i.JobApplication).FirstOrDefault(i => i.InterviewId == id);
            if (interview == null || interview.Status != "Scheduled")
            {
                return NotFound();
            }

            // Update the interview status to "Interviewed"
            interview.Status = "Interviewed";
            interview.JobApplication.Status = "Interviewed"; // Update job application status

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // POST: Interviews/Accept/5
        [HttpPost]
        public IActionResult Accept(int id)
        {
            var interview = _context.Interviews.Include(i => i.JobApplication).FirstOrDefault(i => i.InterviewId == id);
            if (interview == null || interview.Status != "Need Decision")
            {
                return NotFound();
            }

            // Update the interview status to "Accepted"
            interview.Status = "Accepted";
            interview.JobApplication.Status = "Accepted"; // Update job application status

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // POST: Interviews/Reject/5
        [HttpPost]
        public IActionResult Reject(int id)
        {
            var interview = _context.Interviews.Include(i => i.JobApplication).FirstOrDefault(i => i.InterviewId == id);
            if (interview == null || interview.Status != "Need Decision")
            {
                return NotFound();
            }

            // Update the interview status to "Rejected"
            interview.Status = "Rejected";
            interview.JobApplication.Status = "Rejected"; // Update job application status

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}