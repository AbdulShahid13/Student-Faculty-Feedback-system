using FeedbackSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace FeedbackSystem.Controllers
{
    public class TeacherController : Controller
    {
        private readonly FeedbackContext _context;

        public TeacherController(FeedbackContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Teacher")
            {
                return RedirectToAction("TeacherLogin", "Auth");
            }

            var teacherId = HttpContext.Session.GetString("TeacherId");
            Console.WriteLine($"Looking for teacher with TeacherId: {teacherId}");
            
            var teacher = await _context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.TeacherId == teacherId);

            if (teacher == null)
            {
                Console.WriteLine($"Teacher not found with TeacherId: {teacherId}");
                return RedirectToAction("TeacherLogin", "Auth");
            }

            Console.WriteLine($"Found teacher: ID={teacher.Id}, TeacherId={teacher.TeacherId}, Name={teacher.FullName}");

            // Load feedbacks for the teacher using the numeric ID
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Student)
                .Where(f => f.TeacherId == teacher.Id)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            Console.WriteLine($"Found {feedbacks.Count} feedbacks for teacher {teacher.FullName}");
            foreach (var feedback in feedbacks)
            {
                Console.WriteLine($"Feedback: ID={feedback.Id}, StudentId={feedback.StudentId}, TeacherId={feedback.TeacherId}, Semester={feedback.Semester}");
                if (feedback.Student != null)
                {
                    Console.WriteLine($"Student: ID={feedback.Student.Id}, Name={feedback.Student.FullName}");
                }
                else
                {
                    Console.WriteLine("Student information not loaded");
                }
            }

            ViewBag.TeacherName = teacher.FullName;
            ViewBag.Department = teacher.Department;
            ViewBag.TeacherSubjects = teacher.TeacherSubjects;

            return View(feedbacks);
        }

        public async Task<IActionResult> ViewFeedbackBySemester(int semester)
        {
            if (HttpContext.Session.GetString("UserType") != "Teacher")
            {
                return RedirectToAction("TeacherLogin", "Auth");
            }

            var teacherId = HttpContext.Session.GetString("TeacherId");
            Console.WriteLine($"Looking for teacher with TeacherId: {teacherId} and semester: {semester}");
            
            var teacher = await _context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.TeacherId == teacherId);

            if (teacher == null)
            {
                Console.WriteLine($"Teacher not found with TeacherId: {teacherId}");
                return RedirectToAction("TeacherLogin", "Auth");
            }

            Console.WriteLine($"Found teacher: ID={teacher.Id}, TeacherId={teacher.TeacherId}, Name={teacher.FullName}");

            // Load feedbacks for the teacher using the numeric ID and semester
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Student)
                .Where(f => f.TeacherId == teacher.Id && f.Semester == semester)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            Console.WriteLine($"Found {feedbacks.Count} feedbacks for teacher {teacher.FullName} in semester {semester}");
            foreach (var feedback in feedbacks)
            {
                Console.WriteLine($"Feedback: ID={feedback.Id}, StudentId={feedback.StudentId}, TeacherId={feedback.TeacherId}, Semester={feedback.Semester}");
                if (feedback.Student != null)
                {
                    Console.WriteLine($"Student: ID={feedback.Student.Id}, Name={feedback.Student.FullName}");
                }
                else
                {
                    Console.WriteLine("Student information not loaded");
                }
            }

            ViewBag.TeacherName = teacher.FullName;
            ViewBag.Department = teacher.Department;
            ViewBag.TeacherSubjects = teacher.TeacherSubjects;
            ViewBag.CurrentSemester = semester;

            return View("Index", feedbacks);
        }
    }
}