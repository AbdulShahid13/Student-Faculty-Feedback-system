using FeedbackSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeedbackSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly FeedbackContext _context;

        public AuthController(FeedbackContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StudentLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StudentLogin(string registerNumber, string password)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.RegisterNumber == registerNumber && s.Password == password);

            if (student == null)
            {
                TempData["Error"] = "Invalid register number or password";
                return RedirectToAction("StudentLogin");
            }

            HttpContext.Session.SetString("StudentId", student.Id.ToString());
            HttpContext.Session.SetString("StudentName", student.FullName);
            HttpContext.Session.SetString("UserType", "Student");

            return RedirectToAction("SemesterSelection", "Student");
        }

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
                    TempData["Error"] = "Register number already exists";
                    TempData["ActiveTab"] = "signup";
                    return RedirectToAction("StudentLogin");
                }

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("StudentLogin");
            }

            TempData["ActiveTab"] = "signup";
            return RedirectToAction("StudentLogin");
        }

        public IActionResult AdminLogin()
        {
            return View(new AdminLoginViewModel { Username = "", Password = "" });
        }

        [HttpPost]
        public IActionResult AdminLogin(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var admin = _context.Admins.FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);
            if (admin == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            HttpContext.Session.SetString("AdminId", admin.Id.ToString());
            HttpContext.Session.SetString("AdminName", admin.Name);
            HttpContext.Session.SetString("UserType", "Admin");

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult TeacherLogin()
        {
            return View(new TeacherLoginViewModel { TeacherId = "", Password = "" });
        }

        [HttpPost]
        public async Task<IActionResult> TeacherLogin(TeacherLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.TeacherId == model.TeacherId && t.Password == model.Password);

            if (teacher == null)
            {
                TempData["Error"] = "Invalid teacher ID or password";
                return View(model);
            }

            HttpContext.Session.SetString("TeacherId", teacher.TeacherId);
            HttpContext.Session.SetString("TeacherName", teacher.FullName);
            HttpContext.Session.SetString("UserType", "Teacher");

            return RedirectToAction("Index", "Teacher");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
