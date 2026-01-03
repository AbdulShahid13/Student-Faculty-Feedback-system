using Microsoft.AspNetCore.Mvc;
using FeedbackSystem.Models;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly FeedbackContext _context;

    public AccountController(FeedbackContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    // GET: /Account/AdminLogin
    public IActionResult AdminLogin()
    {
        return View();
    }

    // POST: /Account/AdminLogin
    [HttpPost]
    public IActionResult AdminLogin(string username, string password)
    {
        if (username == "admin" && password == "admin123")
        {
            HttpContext.Session.SetString("UserType", "Admin");
            HttpContext.Session.SetString("AdminId", "1");
            return RedirectToAction("Index", "Admin");
        }

        TempData["Error"] = "Invalid username or password";
        return View();
    }

    // GET: /Account/StudentLogin
    public IActionResult StudentLogin()
    {
        return View();
    }

    // POST: /Account/StudentLogin
    [HttpPost]
    public async Task<IActionResult> StudentLogin(string registerNumber, string password)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.RegisterNumber == registerNumber && s.Password == password);

        if (student == null)
        {
            TempData["Error"] = "Invalid register number or password";
            return View();
        }

        HttpContext.Session.SetString("StudentId", student.Id.ToString());
        HttpContext.Session.SetString("StudentName", student.FullName);
        HttpContext.Session.SetString("UserType", "Student");

        return RedirectToAction("Index", "Student");
    }

    // POST: /Account/StudentSignup
    [HttpPost]
    public async Task<IActionResult> StudentSignup(Student student)
    {
        if (ModelState.IsValid)
        {
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.RegisterNumber == student.RegisterNumber);

            if (existingStudent != null)
            {
                ModelState.AddModelError("RegisterNumber", "Register number already exists");
                TempData["ActiveTab"] = "signup";
                return RedirectToAction("Index", "Home");
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Please login with your credentials.";
            return RedirectToAction("Index", "Home");
        }

        foreach (var modelState in ModelState.Values)
        {
            foreach (var error in modelState.Errors)
            {
                ModelState.AddModelError("", error.ErrorMessage);
            }
        }
        TempData["ActiveTab"] = "signup";
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}