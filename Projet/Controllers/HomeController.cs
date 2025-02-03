using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet.Data;
using Projet.Models;

namespace Projet.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: HomeController
        public async Task<IActionResult> Index()
        {
            // Nombre de postes ouverts
            var openPositions = await _context.Jobs.CountAsync();

            // Nombre total de candidatures
            var totalApplications = await _context.JobApplications.CountAsync();

            // Réunions prévues aujourd'hui
            var todayMeetings = await _context.Interviews
                .CountAsync(i => i.ScheduledDate.Date == DateTime.Now.Date);

            // Candidatures à programmer
            var applicationsToSchedule = await _context.JobApplications
        .Where(a => a.Status == "pending" &&
                    !_context.Interviews.Any(i => i.JobApplicationId == a.JobApplicationId))
        .CountAsync();


            var todayInterviews = await _context.Interviews.Include(i => i.JobApplication).ThenInclude(ja => ja.Job)

               .Where(i => i.ScheduledDate.Date == DateTime.Now.Date)
               .ToListAsync();
           var tomorrowInterviews = await _context.Interviews
    .Include(i => i.JobApplication)
        .ThenInclude(ja => ja.Job)
    .Where(i => i.ScheduledDate.Date == DateTime.Now.AddDays(1).Date) // Adjust for tomorrow
    .ToListAsync();
            var jobAppToEvaluate = await _context.Interviews
    .Where(i => i.ScheduledDate < DateTime.Now &&
                i.Status == "pending" &&
                _context.JobApplications
                    .Any(j => j.JobApplicationId == i.JobApplicationId && j.Status == "pending"))
    .CountAsync();
            var interviewsToEvaluate = await _context.Interviews
   .Where(i => i.ScheduledDate < DateTime.Now &&
               i.Status == "pending" )
   .CountAsync();
            var model = new HomeViewModel
            {
                OpenPositions = openPositions,
                TotalApplications = totalApplications,
                TodayMeetings = todayMeetings,
                ApplicationsToSchedule = applicationsToSchedule,
                TodayInterviews = todayInterviews,
                TomorrowInterviews= tomorrowInterviews,
                JobAppToEvaluate= jobAppToEvaluate,
                InterviewsToEvalute= interviewsToEvaluate

            };

            return View(model);
        }
        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
