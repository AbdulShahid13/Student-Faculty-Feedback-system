using FeedbackSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeedbackSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly FeedbackContext _context;

        public AdminController(FeedbackContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }
            return View();
        }

        public IActionResult AddTeacher()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }
            var viewModel = new AddTeacherViewModel
            {
                TeacherId = "",
                Password = "",
                FullName = "",
                Department = "",
                Email = "",
                SubjectSemesters = new List<SubjectSemesterEntry> 
                { 
                    new SubjectSemesterEntry 
                    { 
                        Subject = "",
                        Semester = 1
                    } 
                }
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddTeacher(AddTeacherViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var teacher = new Teacher
            {
                TeacherId = model.TeacherId,
                Password = model.Password,
                FullName = model.FullName,
                Department = model.Department,
                Email = model.Email,
                CreatedAt = DateTime.Now
            };

            foreach (var entry in model.SubjectSemesters)
            {
                teacher.TeacherSubjects.Add(new TeacherSubject
                {
                    Subject = entry.Subject,
                    Semester = entry.Semester
                });
            }

            _context.Teachers.Add(teacher);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Teacher added successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult ManageTeachers()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            var teachers = _context.Teachers
                .Include(t => t.TeacherSubjects)
                .OrderBy(t => t.Department)
                .ToList();
            return View(teachers);
        }

        public IActionResult ViewTeacherFeedback()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            var feedbacks = _context.Feedbacks
                .Include(f => f.Teacher)
                .ToList();

            return View(feedbacks);
        }

        public IActionResult ViewTeacherFeedbackDetails(int teacherId)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            var feedbacks = _context.Feedbacks
                .Include(f => f.Teacher)
                .Where(f => f.TeacherId == teacherId)
                .OrderByDescending(f => f.CreatedAt)
                .ToList();

            if (!feedbacks.Any())
            {
                return NotFound();
            }

            ViewBag.TeacherName = feedbacks.First().Teacher?.FullName;
            return View(feedbacks);
        }

        public IActionResult StudentFeedbacks()
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            var feedbacks = _context.Feedbacks
                .Include(f => f.Teacher)
                .OrderByDescending(f => f.CreatedAt)
                .ToList()
                .Select(f => new
                {
                    TeacherName = f.Teacher != null ? f.Teacher.FullName : "Unknown",
                    Department = f.Department,
                    KnowledgeRating = f.KnowledgeInSubject,
                    ExpressIdeasRating = f.AbilityToExpressIdeas,
                    ExamplesRating = f.AbilityToGiveExamples,
                    InterestRating = f.AbilityToArouseInterest,
                    AttentionRating = f.AbilityToAttractAttention,
                    BehaviorRating = f.GeneralBehavior,
                    HelpRating = f.WillingnessToHelp,
                    EnglishRating = f.CommandOfEnglish,
                    PunctualityRating = f.Punctuality,
                    PreparationRating = f.PreparationAndSincerity,
                    Comments = f.Comments,
                    Semester = f.Semester,
                    CreatedAt = f.CreatedAt
                })
                .ToList()
                .Select(f => new StudentFeedbackViewModel
                {
                    TeacherName = f.TeacherName,
                    Department = f.Department,
                    KnowledgeRating = f.KnowledgeRating,
                    ExpressIdeasRating = f.ExpressIdeasRating,
                    ExamplesRating = f.ExamplesRating,
                    InterestRating = f.InterestRating,
                    AttentionRating = f.AttentionRating,
                    BehaviorRating = f.BehaviorRating,
                    HelpRating = f.HelpRating,
                    EnglishRating = f.EnglishRating,
                    PunctualityRating = f.PunctualityRating,
                    PreparationRating = f.PreparationRating,
                    Comments = f.Comments ?? "No comments provided",
                    Semester = f.Semester,
                    SubmittedDate = f.CreatedAt.ToString("dd MMM yyyy")
                })
                .ToList();

            return View(feedbacks);
        }

        public IActionResult EditTeacher(int id)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            var teacher = _context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefault(t => t.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            var viewModel = new AddTeacherViewModel
            {
                Id = teacher.Id,
                TeacherId = teacher.TeacherId,
                Password = teacher.Password,
                FullName = teacher.FullName,
                Department = teacher.Department,
                Email = teacher.Email,
                SubjectSemesters = teacher.TeacherSubjects.Select(ts => new SubjectSemesterEntry
                {
                    Subject = ts.Subject,
                    Semester = ts.Semester
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditTeacher(int id, AddTeacherViewModel model)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("AdminLogin", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var teacher = _context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefault(t => t.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            // Update teacher details
            teacher.TeacherId = model.TeacherId;
            teacher.Password = model.Password;
            teacher.FullName = model.FullName;
            teacher.Department = model.Department;
            teacher.Email = model.Email;

            // Remove existing subject-semester entries
            _context.TeacherSubjects.RemoveRange(teacher.TeacherSubjects);

            // Add new subject-semester entries
            foreach (var entry in model.SubjectSemesters)
            {
                teacher.TeacherSubjects.Add(new TeacherSubject
                {
                    Subject = entry.Subject,
                    Semester = entry.Semester
                });
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Teacher updated successfully!";
            return RedirectToAction("ManageTeachers");
        }
    }
}