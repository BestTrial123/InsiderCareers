using Microsoft.AspNetCore.Mvc;
using InsiderCareers.Models;
using Microsoft.EntityFrameworkCore;

namespace InsiderCareers.Controllers
{
    public class JobsController : Controller
    {
        private readonly AppDbContext _context;
        public JobsController(AppDbContext context) => _context = context;

        // GET /Jobs?search=...&country=...
        public async Task<IActionResult> Index(string? search, string? country)
        {
            var query = _context.Jobs.Include(j => j.Employer).Where(j => j.Status == "Open").AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(j => j.Title.Contains(search));

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(j => j.Location != null && j.Location.Contains(country));

            var jobs = await query.OrderByDescending(j => j.PostedDate).ToListAsync();
            return View(jobs);
        }
// GET /Jobs/Details/5
public async Task<IActionResult> Details(int id)
{
    var job = await _context.Jobs.Include(j => j.Employer)
        .FirstOrDefaultAsync(j => j.Id == id);

    if (job == null) return NotFound();

    job.ClickCount++;
    await _context.SaveChangesAsync();

    return View(job);
}

        // GET /Jobs/Post
        [HttpGet]
        public IActionResult Post() => View();

        // POST /Jobs/Post
        [HttpPost]
        public async Task<IActionResult> Post(Job job)
        {
            // TEMP: hardcode EmployerId until real login/session is wired
            var employer = await _context.Employers.FirstOrDefaultAsync();
            if (employer == null) return RedirectToAction("RegisterEmployer", "Account");

            job.EmployerId = employer.Id;
            job.PostedDate = DateTime.UtcNow;
            job.Status = "Open";

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return RedirectToAction("EmployerDashboard", "Account");
        }
    // POST /Jobs/Apply
[HttpPost]
public async Task<IActionResult> Apply(int jobId)
{
    // TEMP: hardcode JobSeekerId until real login/session is wired
    var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync();
    if (jobSeeker == null) return RedirectToAction("RegisterJobseeker", "Account");

    var alreadyApplied = await _context.JobApplications
        .AnyAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeeker.Id);

    if (!alreadyApplied)
    {
        var application = new JobApplication
        {
            JobId = jobId,
            JobSeekerId = jobSeeker.Id,
            AppliedDate = DateTime.UtcNow,
            Status = "Applied"
        };
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();
    }

    return RedirectToAction("JobSeekerDashboard", "Account");
}
    }
}