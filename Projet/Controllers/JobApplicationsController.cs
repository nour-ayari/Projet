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
using System.Net.Mail;
using Microsoft.Build.Evaluation;

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
        public IActionResult Index(string filter, string jobname)
        {
            var jobApplications = _context.JobApplications.Include(ja => ja.Job).ToList();

            if (!string.IsNullOrEmpty(jobname))
            {
                jobApplications = jobApplications.Where(ja => ja.Job.Title == Uri.UnescapeDataString(jobname)).ToList();
            }

            if (filter == "schedule")
            {
                jobApplications = jobApplications.Where(ja => ja.Status == "Pending").ToList();
            }

            ViewBag.JobTitles = _context.Jobs.Select(j => j.Title).Distinct().ToList();

            return View(jobApplications);
        }

        // Schedule Interview - Create a new Interview with a scheduled date and meeting link
        public async Task<IActionResult> Schedule(int id)
        {
            var jobApplication = _context.JobApplications
                .FirstOrDefault(ja => ja.JobApplicationId == id);

            if (jobApplication == null || jobApplication.Status != "Pending")
            {
                return NotFound();
            }

            var viewModel = new ScheduleInterviewViewModel
            {
                JobApplicationId = jobApplication.JobApplicationId
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Schedule(ScheduleInterviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var jobApplication = _context.JobApplications
                .FirstOrDefault(ja => ja.JobApplicationId == model.JobApplicationId);

            if (jobApplication == null || jobApplication.Status != "Pending")
            {
                return NotFound();
            }

            // Change the status of the job application to 'Scheduled'
            jobApplication.Status = "Scheduled";
            _context.SaveChanges();

            // Create a new interview
            var interview = new Interview
            {
                JobApplicationId = jobApplication.JobApplicationId,
                ScheduledDate = model.InterviewDate,
                Status = "Scheduled",
                Feedback = string.Empty,
                meet = model.meet
            };

            _context.Interviews.Add(interview);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); 
        }

        /*private async Task SendInterviewEmailAsync(string applicantEmail, DateTime interviewDate, string meetingLink)
        {
            var subject = "Your Interview has been Scheduled";
            var body = $@"
            Dear Applicant,

            We are pleased to inform you that your interview has been scheduled. Here are the details:

            Interview Date: {interviewDate:yyyy-MM-dd HH:mm}
            Meeting Link: {meetingLink}

            Please ensure that you join the meeting on time.

            Best regards,
            The Recruitment Team
        ";

            using (var message = new MailMessage())
            {
                var mail = "servicerhdotnet@gmail.com";
                message.To.Add(applicantEmail);
                message.From = new MailAddress(mail);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false;

                using (var smtpClient = new SmtpClient("smtp-mail.outlook.com"))
                {
                    smtpClient.Port = 587; 
                    smtpClient.Credentials = new System.Net.NetworkCredential("servicerhdotnet@gmail.com", "Projet.Net");
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(message);
                }
            }
        }*/

        // Reject - Change status to rejected
        public IActionResult Reject(int id)
            {
                var jobApplication = _context.JobApplications
                    .FirstOrDefault(ja => ja.JobApplicationId == id);

                if (jobApplication != null)
                {
                    jobApplication.Status = "Rejected";
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
   

        // Accept - Change status to accepted
        public IActionResult Accept(int id)
            {
                var jobApplication = _context.JobApplications
                    .FirstOrDefault(ja => ja.JobApplicationId == id);

                if (jobApplication != null && jobApplication.Status == "Interviewed")
                {
                    jobApplication.Status = "Accepted";
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }

            
        }
    }

