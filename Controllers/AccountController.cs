using Microsoft.AspNetCore.Mvc;
using InsiderCareers.Models;
using Microsoft.EntityFrameworkCore;
using InsiderCareers.Services;
namespace InsiderCareers.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AccountController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Appointments()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

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
                HttpContext.Session.SetString("UserName", jobSeeker.FirstName + " " + jobSeeker.LastName);
                HttpContext.Session.SetInt32("JobSeekerId", jobSeeker.Id);
                HttpContext.Session.SetString("UserType", "JobSeeker");
                HttpContext.Session.SetString("UserEmail", jobSeeker.Email);
                HttpContext.Session.SetString("UserPhone", jobSeeker.Phone ?? "");
                return RedirectToAction("JobSeekerDashboard");
            }

            var employer = await _context.Employers
                .FirstOrDefaultAsync(e => e.Email == Email && e.Password == Password);

            if (employer != null)
            {
                HttpContext.Session.SetString("UserName", employer.CompanyName);
                HttpContext.Session.SetInt32("EmployerId", employer.Id);
                HttpContext.Session.SetString("UserType", "Employer");
                HttpContext.Session.SetString("UserContactName", employer.ContactName);
                HttpContext.Session.SetString("UserEmail", employer.Email);
                return RedirectToAction("EmployerDashboard");
            }
var admin = await _context.Admins
    .FirstOrDefaultAsync(a => a.Email == Email && a.Password == Password);

if (admin != null)
{
    HttpContext.Session.SetString("UserName" , admin.FullName);
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
    ViewBag.JobClicks = jobs.Select(j => j.ClickCount).ToList();

var pendingEmployers = await _context.Employers
    .CountAsync(e => !e.InterviewStatus);
ViewBag.PendingEmployers = pendingEmployers;

var recentJobs = await _context.Jobs
    .Include(j => j.Employer)
    .OrderByDescending(j => j.Id)
    .Take(5)
    .ToListAsync();
ViewBag.RecentJobs = recentJobs;

var recentEmployers = await _context.Employers
    .OrderByDescending(e => e.Id)
    .Take(5)
    .ToListAsync();
ViewBag.RecentEmployers = recentEmployers;

return View();
}

[HttpGet]
public async Task<IActionResult> VerifyEmployers()
{
    var employers = await _context.Employers.ToListAsync();
    return View(employers);
}

[HttpPost]
public async Task<IActionResult> ToggleVerification(int id)
{
    var employer = await _context.Employers.FindAsync(id);
    if (employer != null)
    {
            employer.InterviewStatus = !employer.InterviewStatus;
            await _context.SaveChangesAsync();
        if (employer.InterviewStatus)
        {
            await _emailService.SendEmailAsync(
                employer.Email,
                "You're verified on Insider Careers",
                $"<p>Hi {employer.ContactName}, your employer account for {employer.CompanyName} has been verified. You can now post jobs and message candidates.</p>"
            );
        }
    }
    return RedirectToAction("VerifyEmployers");
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
            ViewData["EmployerVerified"] = employer?.InterviewStatus ?? false;

    return View(jobs);
}

        [HttpGet]
public async Task<IActionResult> Candidates()
{
    var employer = await _context.Employers.FirstOrDefaultAsync();
    var jobs = await _context.Jobs
    .Where(j => employer != null && j.EmployerId == employer.Id)
    .Include(j => j.Applications)
        .ThenInclude(a => a.JobSeeker)
    .OrderByDescending(j => j.PostedDate)
    .ToListAsync();

    return View(jobs);
}
[HttpGet]
public async Task<IActionResult> Calendar()
{
    ViewData["Title"] = "Calendar";
    var employer = await _context.Employers.FirstOrDefaultAsync();
    var interviews = await _context.JobApplications
        .Include(a => a.Job)
        .Include(a => a.JobSeeker)
        .Where(a => a.Job != null && employer != null && a.Job.EmployerId == employer.Id && a.Status == "Interview")
        .OrderBy(a => a.InterviewDate)
        .ToListAsync();
    return View(interviews);
}
[HttpGet]
public async Task<IActionResult> Message(int? id)
{
    var userType = HttpContext.Session.GetString("UserType");
    int? currentId = userType == "Employer"
        ? HttpContext.Session.GetInt32("EmployerId")
        : HttpContext.Session.GetInt32("JobSeekerId");

    if (userType == null || currentId == null)
        return RedirectToAction("Login");

    var messages = await _context.Messages
        .Where(m => (m.RecipientType == userType && m.RecipientId == currentId)
                 || (m.SenderType == userType && m.SenderId == currentId))
        .OrderByDescending(m => m.SentDate)
        .ToListAsync();

    ViewData["SelectedId"] = id;
    return View(messages);
}
[HttpGet]
public async Task<IActionResult> Compose(int? recipientId, string? recipientType)
{
    var userType = HttpContext.Session.GetString("UserType");
    int? currentId = userType == "Employer"
        ? HttpContext.Session.GetInt32("EmployerId")
        : HttpContext.Session.GetInt32("JobSeekerId");

    if (userType == null || currentId == null)
        return RedirectToAction("Login");

    string recipientName = "";
    string recipientEmail = "";

    if (recipientId.HasValue && recipientType != null)
    {
        if (recipientType == "Employer")
        {
            var employer = await _context.Employers.FindAsync(recipientId.Value);
            if (employer != null)
            {
                recipientName = employer.ContactName;
                recipientEmail = employer.Email;
            }
        }
        else if (recipientType == "JobSeeker")
        {
            var jobSeeker = await _context.JobSeekers.FindAsync(recipientId.Value);
            if (jobSeeker != null)
            {
                recipientName = $"{jobSeeker.FirstName} {jobSeeker.LastName}";
                recipientEmail = jobSeeker.Email;
            }
        }
    }

    ViewData["RecipientId"] = recipientId;
    ViewData["RecipientType"] = recipientType;
    ViewData["RecipientName"] = recipientName;
    ViewData["RecipientEmail"] = recipientEmail;

    return View();
}
[HttpPost]
public async Task<IActionResult> Compose(int recipientId, string recipientType, string subject, string body, IFormFile? attachment)
{
    var userType = HttpContext.Session.GetString("UserType");
    int? currentId = userType == "Employer"
        ? HttpContext.Session.GetInt32("EmployerId")
        : HttpContext.Session.GetInt32("JobSeekerId");

    if (userType == null || currentId == null)
        return RedirectToAction("Login");

    string senderName = "";

    if (userType == "Employer")
    {
        var employer = await _context.Employers.FindAsync(currentId.Value);
        senderName = employer?.ContactName ?? "";
    }
    else if (userType == "JobSeeker")
    {
        var jobSeeker = await _context.JobSeekers.FindAsync(currentId.Value);
        senderName = jobSeeker != null ? $"{jobSeeker.FirstName} {jobSeeker.LastName}" : "";
    }

    string? attachmentPath = null;
    string? attachmentFileName = null;

    if (attachment != null && attachment.Length > 0)
    {
        var uploadsFolder = Path.Combine("wwwroot", "uploads", "messages");
        Directory.CreateDirectory(uploadsFolder);
        var fileName = $"{Guid.NewGuid()}_{attachment.FileName}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await attachment.CopyToAsync(stream);
        }

        attachmentPath = $"/uploads/messages/{fileName}";
        attachmentFileName = attachment.FileName;
    }

    var message = new Message
    {
        SenderType = userType,
        SenderId = currentId.Value,
        SenderName = senderName,
        RecipientType = recipientType,
        RecipientId = recipientId,
        Subject = subject,
        Body = body,
        SentDate = DateTime.UtcNow,
        IsRead = false,
        AttachmentPath = attachmentPath,
        AttachmentFileName = attachmentFileName
    };

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
public async Task<IActionResult> JobSeekerDashboard(string? q, string? location)
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
    var unreadCount = await _context.Messages
    .CountAsync(m => m.RecipientId == jobSeekerId && m.RecipientType == "JobSeeker" && !m.IsRead);

    var viewModel = new JobSeekerDashboardViewModel
    {
        JobSeeker = jobSeeker,
        AppliedCount = applications.Count(a => a.Status == "Applied"),
        SavedCount = applications.Count(a => a.Status == "Saved"),
        ViewedCount = applications.Count(a => a.Status == "Viewed"),
        DeniedCount = applications.Count(a => a.Status == "Denied"),
        UnreadMessageCount = unreadCount,
        InterviewAppointments = applications
            .Where(a => a.Status == "Interview" && a.InterviewDate != null)
            .OrderBy(a => a.InterviewDate)
            .ToList(),
        JobFeed = await _context.Jobs
            .Include(j => j.Employer)
            .Where(j => j.Status == "Open")
            .Where(j => string.IsNullOrEmpty(q) || j.Title.Contains(q))
            .Where(j => string.IsNullOrEmpty(location) || j.Location!.Contains(location))
            .OrderByDescending(j => j.PostedDate)
            .Take(20)
            .ToListAsync()
    };

    return View(viewModel);
}
// ---------- EMPLOYERS ----------
[HttpGet]
public async Task<IActionResult> ManageEmployers()
{
    var employers = await _context.Employers.ToListAsync();
    return View(employers);
}

[HttpGet]
public async Task<IActionResult> EditEmployer(int id)
{
    var employer = await _context.Employers.FindAsync(id);
    if (employer == null) return NotFound();
    return View(employer);
}

[HttpPost]
public async Task<IActionResult> EditEmployer(Employer model)
{
    var employer = await _context.Employers.FindAsync(model.Id);
    if (employer == null) return NotFound();

    employer.CompanyName = model.CompanyName;
    employer.ContactName = model.ContactName;
    employer.Email = model.Email;
    employer.Phone = model.Phone;
    employer.StreetAddress = model.StreetAddress;
    employer.City = model.City;
    employer.State = model.State;
    employer.Zip = model.Zip;
    employer.BusinessType = model.BusinessType;
    employer.EmployeeCount = model.EmployeeCount;
    employer.WebsiteUrl = model.WebsiteUrl;
    employer.OperatingRegion = model.OperatingRegion;
    employer.DirectPhone = model.DirectPhone;
    employer.TaxId = model.TaxId;
    employer.ContactPosition = model.ContactPosition;
    employer.InterviewStatus = model.InterviewStatus;

    await _context.SaveChangesAsync();
    return RedirectToAction("ManageEmployers");
}

[HttpPost]
public async Task<IActionResult> DeleteEmployer(int id)
{
    var employer = await _context.Employers.FindAsync(id);
    if (employer != null)
    {
        _context.Employers.Remove(employer);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction("ManageEmployers");
}

// ---------- JOB SEEKERS ----------
[HttpGet]
public async Task<IActionResult> ManageJobSeekers()
{
    var jobSeekers = await _context.JobSeekers.ToListAsync();
    return View(jobSeekers);
}

[HttpGet]
public async Task<IActionResult> EditJobSeeker(int id)
{
    var jobSeeker = await _context.JobSeekers.FindAsync(id);
    if (jobSeeker == null) return NotFound();
    return View(jobSeeker);
}

[HttpPost]
public async Task<IActionResult> EditJobSeeker(JobSeeker model)
{
    var jobSeeker = await _context.JobSeekers.FindAsync(model.Id);
    if (jobSeeker == null) return NotFound();

    jobSeeker.FirstName = model.FirstName;
    jobSeeker.LastName = model.LastName;
    jobSeeker.Email = model.Email;
    jobSeeker.Phone = model.Phone;
    jobSeeker.DateOfBirth = model.DateOfBirth;
    jobSeeker.Gender = model.Gender;
    jobSeeker.MaritalStatus = model.MaritalStatus;
    jobSeeker.LanguageSpoken = model.LanguageSpoken;
    jobSeeker.Skills = model.Skills;
    jobSeeker.WorkHistory = model.WorkHistory;
    jobSeeker.SalaryDesire = model.SalaryDesire;
    jobSeeker.Address = model.Address;
    jobSeeker.Country = model.Country;
    jobSeeker.City = model.City;

    await _context.SaveChangesAsync();
    return RedirectToAction("ManageJobSeekers");
}

[HttpPost]
public async Task<IActionResult> DeleteJobSeeker(int id)
{
    var jobSeeker = await _context.JobSeekers.FindAsync(id);
    if (jobSeeker != null)
    {
        _context.JobSeekers.Remove(jobSeeker);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction("ManageJobSeekers");
}
    }
}