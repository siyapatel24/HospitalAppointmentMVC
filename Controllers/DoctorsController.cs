using Microsoft.AspNetCore.Mvc;
using HospitalAppointmentMVC.Services;

namespace HospitalAppointmentMVC.Controllers;

public class DoctorsController(
    DoctorService doctorService,
    IHttpClientFactory httpClientFactory,
    ILogger<DoctorsController> logger) : Controller
{
    public IActionResult Index()
    {
        // Cookie: remember the patient's name for later visits.
        var patientName = Request.Cookies["PatientName"];
        ViewBag.PatientName = patientName;
        ViewBag.SelectedSpecialization = HttpContext.Session.GetString("SelectedSpecialization");
        return View(doctorService.GetDoctors());
    }

    [HttpGet]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false, VaryByQueryKeys = new[] { "id" })]
    public IActionResult Details(int id)
    {
        // DoctorId is passed through the query string: /Doctors/Details?id=1
        var doctor = doctorService.GetDoctorById(id);
        if (doctor is null) return NotFound();

        ViewBag.PatientName = Request.Cookies["PatientName"];
        return View(doctor);
    }

    [HttpGet]
    public IActionResult Appointment(int doctorId)
    {
        var doctor = doctorService.GetDoctorById(doctorId);
        if (doctor is null) return NotFound();

        HttpContext.Session.SetString("SelectedSpecialization", doctor.Specialization);
        ViewBag.Doctor = doctor;
        return View(new AppointmentRequest { DoctorId = doctor.DoctorId, AppointmentDate = DateTime.Today.AddDays(1) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Appointment(AppointmentRequest request)
    {
        var doctor = doctorService.GetDoctorById(request.DoctorId);
        if (doctor is null) return NotFound();

        if (request.AppointmentDate.Date < DateTime.Today)
            ModelState.AddModelError(nameof(request.AppointmentDate), "Appointment date cannot be in the past.");

        if (!ModelState.IsValid)
        {
            ViewBag.Doctor = doctor;
            return View(request);
        }

        // Cookie: store patient name for the next visit.
        Response.Cookies.Append("PatientName", request.PatientName, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = Request.IsHttps
        });

        // Session: store selected specialization.
        HttpContext.Session.SetString("SelectedSpecialization", doctor.Specialization);

        TempData["SuccessMessage"] = $"Appointment request received for {request.PatientName} with {doctor.DoctorName} on {request.AppointmentDate:dd MMM yyyy}. Total charge: ₹{doctor.CalculateTotalCharge():N2}.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Posts(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(15);

        try
        {
            var posts = await client.GetFromJsonAsync<List<JsonPost>>(
                "https://jsonplaceholder.typicode.com/posts", cancellationToken);
            return View(posts ?? new List<JsonPost>());
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Could not retrieve posts from JSONPlaceholder.");
            ViewBag.ErrorMessage = "Posts could not be loaded right now. Please try again later.";
            return View(new List<JsonPost>());
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(ex, "The JSONPlaceholder request timed out.");
            ViewBag.ErrorMessage = "The request timed out. Please try again later.";
            return View(new List<JsonPost>());
        }
    }
}
