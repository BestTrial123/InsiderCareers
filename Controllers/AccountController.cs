using Microsoft.AspNetCore.Mvc;
using InsiderCareers.Models;
using Microsoft.EntityFrameworkCore;
namespace InsiderCareers.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
public AccountController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

                [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(js => js.Email == Email && js.Password == Password);

            if (jobSeeker != null)
            {
                HttpContext.Session.SetInt32("JobSeekerId", jobSeeker.Id);
                HttpContext.Session.SetString("UserType", "JobSeeker");
                return RedirectToAction("JobSeekerDashboard");
            }

            var employer = await _context.Employers
                .FirstOrDefaultAsync(e => e.Email == Email && e.Password == Password);

            if (employer != null)
            {
                HttpContext.Session.SetInt32("EmployerId", employer.Id);
                HttpContext.Session.SetString("UserType", "Employer");
                return RedirectToAction("EmployerDashboard");
            }
var admin = await _context.Admins
    .FirstOrDefaultAsync(a => a.Email == Email && a.Password == Password);

if (admin != null)
{
    HttpContext.Session.SetInt32("AdminId", admin.Id);
    HttpContext.Session.SetString("UserType", "Admin");
    return RedirectToAction("AdminDashboard");
}

            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }
        [HttpGet]
public IActionResult RegisterAdmin()
{
    return View();
}
[HttpGet]
public async Task<IActionResult> AdminDashboard()
{
    if (HttpContext.Session.GetString("UserType") != "Admin")
    {
        return RedirectToAction("Login");
    }

    var adminId = HttpContext.Session.GetInt32("AdminId");
    var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Id == adminId);

    decimal ratePerClick = 0.15m;

    var jobs = await _context.Jobs
        .OrderByDescending(j => j.ClickCount)
        .Take(6)
        .ToListAsync();

    var totalClicks = await _context.Jobs.SumAsync(j => j.ClickCount);
    var totalJobs = await _context.Jobs.CountAsync();
    var totalEarnings = totalClicks * ratePerClick;

    ViewBag.AdminName = admin?.FullName;
    ViewBag.AdminEmail = admin?.Email;
    ViewBag.TotalClicks = totalClicks;
    ViewBag.TotalJobs = totalJobs;
    ViewBag.RatePerClick = ratePerClick;
    ViewBag.TotalEarnings = totalEarnings;
    ViewBag.JobLabels = jobs.Select(j => j.Title).ToList();
    ViewBag.JobClicks = jobs.Select(j => j.ClickCount).ToList();

    return View();
}
   [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
public IActionResult RegisterJobseeker()
{
    return View();
}
[HttpPost]
public async Task<IActionResult> RegisterJobseeker(JobSeeker model, IFormFile? photo, IFormFile? resume)
{
    if (!ModelState.IsValid)
        return View(model);

    // Save photo
    if (photo != null && photo.Length > 0)
    {
        var photoFolder = Path.Combine("wwwroot", "uploads", "photos");
        Directory.CreateDirectory(photoFolder);
        var photoFileName = $"{Guid.NewGuid()}_{photo.FileName}";
        var photoFullPath = Path.Combine(photoFolder, photoFileName);
        using (var stream = new FileStream(photoFullPath, FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }
        model.PhotoPath = $"/uploads/photos/{photoFileName}";
    }

    // Save resume
    if (resume != null && resume.Length > 0)
    {
        var resumeFolder = Path.Combine("wwwroot", "uploads", "resumes");
        Directory.CreateDirectory(resumeFolder);
        var resumeFileName = $"{Guid.NewGuid()}_{resume.FileName}";
        var resumeFullPath = Path.Combine(resumeFolder, resumeFileName);
        using (var stream = new FileStream(resumeFullPath, FileMode.Create))
        {
            await resume.CopyToAsync(stream);
        }
        model.ResumePath = $"/uploads/resumes/{resumeFileName}";
    }

    _context.JobSeekers.Add(model);
    await _context.SaveChangesAsync();

    // TODO: redirect to a Jobseeker dashboard once it exists
    return RedirectToAction("Login");
}

        [HttpGet]
        public IActionResult RegisterEmployer()
        {
            return View();
        }

       [HttpPost]
public async Task<IActionResult> RegisterEmployer(Employer model, IFormFile? logo, IFormFile? verificationDocument)
{
    if (logo != null && logo.Length > 0)
    {
        var logoFileName = Guid.NewGuid() + Path.GetExtension(logo.FileName);
        var logoPath = Path.Combine("wwwroot/Images/uploads", logoFileName);
        Directory.CreateDirectory(Path.GetDirectoryName(logoPath)!);
        using (var stream = new FileStream(logoPath, FileMode.Create))
        {
            await logo.CopyToAsync(stream);
        }
        model.LogoPath = "/Images/uploads/" + logoFileName;
    }

    if (verificationDocument != null && verificationDocument.Length > 0)
    {
        var docFileName = Guid.NewGuid() + Path.GetExtension(verificationDocument.FileName);
        var docPath = Path.Combine("wwwroot/uploads/verification", docFileName);
        Directory.CreateDirectory(Path.GetDirectoryName(docPath)!);
        using (var stream = new FileStream(docPath, FileMode.Create))
        {
            await verificationDocument.CopyToAsync(stream);
        }
        model.VerificationDocumentPath = "/uploads/verification/" + docFileName;
    }

    _context.Employers.Add(model);
    await _context.SaveChangesAsync();
    return RedirectToAction("EmployerDashboard");
}

        [HttpGet]
public async Task<IActionResult> EmployerDashboard()
{
    var employer = await _context.Employers.FirstOrDefaultAsync();
    var jobs = await _context.Jobs
        .Where(j => employer != null && j.EmployerId == employer.Id)
        .OrderByDescending(j => j.PostedDate)
        .Include(j => j.Applications)
        .ToListAsync();

    ViewData["ApplicantCount"] = jobs.Sum(j => j.Applications.Count);

    return View(jobs);
}

        [HttpGet]
public async Task<IActionResult> Candidates()
{
    var employer = await _context.Employers.FirstOrDefaultAsync();
    var jobs = await _context.Jobs
        .Where(j => employer != null && j.EmployerId == employer.Id)
        .Include(j => j.Applications)
        .OrderByDescending(j => j.PostedDate)
        .ToListAsync();

    return View(jobs);
}
[HttpGet]
public async Task<IActionResult> Message(int? id)
{
    var employer = await _context.Employers.FirstOrDefaultAsync();
    var messages = await _context.Messages
        .Where(m => employer != null && m.EmployerId == employer.Id)
        .OrderByDescending(m => m.SentDate)
        .ToListAsync();

    ViewData["SelectedId"] = id;
    return View(messages);
}
[HttpGet]
public IActionResult Compose()
{
    return View();
}

[HttpPost]
public async Task<IActionResult> Compose(string recipientEmail, string subject, string body, IFormFile? attachment)
{
    var employer = await _context.Employers.FirstOrDefaultAsync();

    var message = new Message
    {
        EmployerId = employer?.Id ?? 0,
        SenderName = employer?.CompanyName ?? "Insider Careers",
        SenderEmail = employer?.Email ?? "noreply@insidercareers.com",
        RecipientEmail = recipientEmail,
        Subject = subject,
        Body = body,
        SentDate = DateTime.Now,
        IsRead = true
    };

    if (attachment != null && attachment.Length > 0)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(attachment.FileName);
        var filePath = Path.Combine("wwwroot/uploads/messages", fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await attachment.CopyToAsync(stream);
        }
        message.AttachmentPath = "/uploads/messages/" + fileName;
        message.AttachmentFileName = attachment.FileName;
    }

    _context.Messages.Add(message);
    await _context.SaveChangesAsync();

    return RedirectToAction("Message");
}
[HttpGet]
public async Task<IActionResult> JobSeekerProfile()
{
    if (HttpContext.Session.GetString("UserType") != "JobSeeker")
    {
        return RedirectToAction("Login");
    }

    var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId");
    var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync(j => j.Id == jobSeekerId);

    if (jobSeeker == null) return RedirectToAction("Login");

    return View(jobSeeker);
}
[HttpGet]
public async Task<IActionResult> JobSeekerDashboard()
{
                var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId");
            if (jobSeekerId == null)
                return RedirectToAction("Login");

            var jobSeeker = await _context.JobSeekers
                .Include(js => js.Applications)
                    .ThenInclude(a => a.Job)
                        .ThenInclude(j => j!.Employer)
                .FirstOrDefaultAsync(js => js.Id == jobSeekerId);

            if (jobSeeker == null)
                return RedirectToAction("Login");

    var applications = jobSeeker.Applications;

    var viewModel = new JobSeekerDashboardViewModel
    {
        JobSeeker = jobSeeker,
        AppliedCount = applications.Count(a => a.Status == "Applied"),
        SavedCount = applications.Count(a => a.Status == "Saved"),
        ViewedCount = applications.Count(a => a.Status == "Viewed"),
        DeniedCount = applications.Count(a => a.Status == "Denied"),
        InterviewAppointments = applications
            .Where(a => a.Status == "Interview" && a.InterviewDate != null)
            .OrderBy(a => a.InterviewDate)
            .ToList(),
        JobFeed = await _context.Jobs
            .Include(j => j.Employer)
            .Where(j => j.Status == "Open")
            .OrderByDescending(j => j.PostedDate)
            .Take(20)
            .ToListAsync()
    };

    return View(viewModel);
}
    }
}

