using FeedbackSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeedbackSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly FeedbackContext _context;

        public StudentController(FeedbackContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserType") != "Student")
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            var studentIdStr = HttpContext.Session.GetString("StudentId");
            if (string.IsNullOrEmpty(studentIdStr))
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id.ToString() == studentIdStr);
            if (student == null)
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            ViewBag.StudentName = student.FullName;
            ViewBag.Department = student.Department;

            return View();
        }

        public IActionResult SemesterSelection()
        {
            // Check if user is logged in
            if (HttpContext.Session.GetString("StudentId") == null)
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            return View();
        }

        public IActionResult SelectSemester(int semester)
        {
            if (HttpContext.Session.GetString("UserType") != "Student")
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            // Store selected semester in session
            HttpContext.Session.SetInt32("SelectedSemester", semester);
            
            return RedirectToAction("DepartmentSelection");
        }

        public IActionResult DepartmentSelection()
        {
            if (HttpContext.Session.GetString("UserType") != "Student")
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            var semester = HttpContext.Session.GetInt32("SelectedSemester");
            if (!semester.HasValue)
            {
                return RedirectToAction("SemesterSelection");
            }

            ViewBag.SelectedSemester = semester.Value;
            return View();
        }

        [HttpGet]
        public IActionResult SelectDepartment(string department)
        {
            if (HttpContext.Session.GetString("UserType") != "Student")
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            var semester = HttpContext.Session.GetInt32("SelectedSemester");
            if (!semester.HasValue)
            {
                return RedirectToAction("SemesterSelection");
            }

            // Store selected department in session
            HttpContext.Session.SetString("SelectedDepartment", department);

            return RedirectToAction("TeacherSelection");
        }

        public async Task<IActionResult> TeacherSelection()
        {
            if (HttpContext.Session.GetString("UserType") != "Student")
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            var studentIdStr = HttpContext.Session.GetString("StudentId");
            if (string.IsNullOrEmpty(studentIdStr))
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id.ToString() == studentIdStr);
            if (student == null)
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            // Get list of teachers from the same department
            var teachers = await _context.Teachers
                .Where(t => t.Department == student.Department)
                .OrderBy(t => t.FullName)
                .ToListAsync();

            Console.WriteLine($"Found {teachers.Count} teachers for department {student.Department}");
            foreach (var teacher in teachers)
            {
                Console.WriteLine($"Teacher: ID={teacher.Id}, Name={teacher.FullName}, Department={teacher.Department}");
            }

            ViewBag.StudentName = student.FullName;
            ViewBag.Department = student.Department;

            return View(teachers ?? new List<Teacher>());
        }

        [HttpPost]
        public async Task<IActionResult> SelectTeacher(int teacherId, int semester)
        {
            if (HttpContext.Session.GetString("UserType") != "Student")
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            var studentIdStr = HttpContext.Session.GetString("StudentId");
            if (string.IsNullOrEmpty(studentIdStr))
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id.ToString() == studentIdStr);
            if (student == null)
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Invalid teacher selected. Please try again.";
                return RedirectToAction("TeacherSelection");
            }

            // Create a new feedback object
            var feedback = new Feedback
            {
                TeacherId = teacher.Id,
                Department = teacher.Department,
                Semester = semester,
                KnowledgeInSubject = "",
                AbilityToExpressIdeas = "",
                AbilityToGiveExamples = "",
                AbilityToArouseInterest = "",
                AbilityToAttractAttention = "",
                GeneralBehavior = "",
                WillingnessToHelp = "",
                CommandOfEnglish = "",
                Punctuality = "",
                PreparationAndSincerity = ""
            };

            ViewBag.TeacherName = teacher.FullName;
            ViewBag.Department = teacher.Department;
            ViewBag.SelectedSemester = semester;

            return View("GiveFeedback", feedback);
        }

        public IActionResult GiveFeedback(string teacherId)
        {
            if (HttpContext.Session.GetString("UserType") != "Student")
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            var semester = HttpContext.Session.GetInt32("SelectedSemester");
            if (!semester.HasValue)
            {
                return RedirectToAction("SemesterSelection");
            }

            var studentIdStr = HttpContext.Session.GetString("StudentId");
            if (!int.TryParse(studentIdStr, out int studentId))
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            Console.WriteLine($"Student {studentId} attempting to give feedback to teacher {teacherId}");

            var teacher = _context.Teachers.FirstOrDefault(t => t.TeacherId == teacherId);
            if (teacher == null)
            {
                Console.WriteLine($"Teacher not found with TeacherId: {teacherId}");
                return NotFound();
            }

            Console.WriteLine($"Found teacher: ID={teacher.Id}, TeacherId={teacher.TeacherId}, Name={teacher.FullName}");

            // Check if feedback already exists
            var existingFeedback = _context.Feedbacks
                .FirstOrDefault(f => f.StudentId == studentId && 
                                   f.TeacherId == teacher.Id && 
                                   f.Semester == semester.Value);

            if (existingFeedback != null)
            {
                Console.WriteLine($"Found existing feedback for student {studentId}, teacher {teacher.Id}, semester {semester.Value}");
                TempData["Error"] = "You have already submitted feedback for this teacher in this semester.";
                return RedirectToAction("TeacherSelection");
            }

            var feedback = new Feedback
            {
                TeacherId = teacher.Id,
                StudentId = studentId,
                Department = HttpContext.Session.GetString("SelectedDepartment") ?? teacher.Department,
                Semester = semester.Value,
                CreatedAt = DateTime.Now,
                KnowledgeInSubject = "",
                AbilityToExpressIdeas = "",
                AbilityToGiveExamples = "",
                AbilityToArouseInterest = "",
                AbilityToAttractAttention = "",
                GeneralBehavior = "",
                WillingnessToHelp = "",
                CommandOfEnglish = "",
                Punctuality = "",
                PreparationAndSincerity = ""
            };

            Console.WriteLine($"Created new feedback object: TeacherId={feedback.TeacherId}, StudentId={feedback.StudentId}, Semester={feedback.Semester}");

            ViewBag.TeacherName = teacher.FullName;
            ViewBag.Department = feedback.Department;
            ViewBag.SelectedSemester = semester.Value;

            return View(feedback);
        }

        [HttpPost]
        public IActionResult SubmitFeedback(Feedback feedback)
        {
            Console.WriteLine("=== Starting Feedback Submission ===");
            
            try
            {
                if (HttpContext.Session.GetString("UserType") != "Student")
                {
                    Console.WriteLine("Error: User is not logged in as student");
                    TempData["ErrorMessage"] = "Please log in as a student to submit feedback.";
                    return RedirectToAction("StudentLogin", "Auth");
                }

                var studentIdStr = HttpContext.Session.GetString("StudentId");
                if (string.IsNullOrEmpty(studentIdStr))
                {
                    Console.WriteLine("Error: StudentId not found in session");
                    TempData["ErrorMessage"] = "Student ID not found. Please log in again.";
                    return RedirectToAction("StudentLogin", "Auth");
                }

                if (!int.TryParse(studentIdStr, out int studentId))
                {
                    Console.WriteLine($"Error: Invalid StudentId format: {studentIdStr}");
                    TempData["ErrorMessage"] = "Invalid student ID. Please log in again.";
                    return RedirectToAction("StudentLogin", "Auth");
                }

                // Verify teacher exists
                var teacher = _context.Teachers.FirstOrDefault(t => t.Id == feedback.TeacherId);
                if (teacher == null)
                {
                    Console.WriteLine($"Error: Teacher with ID {feedback.TeacherId} not found");
                    TempData["ErrorMessage"] = "Invalid teacher selected. Please try again.";
                    return RedirectToAction("Index", "Student");
                }

                // Verify student exists
                var student = _context.Students.FirstOrDefault(s => s.Id == studentId);
                if (student == null)
                {
                    Console.WriteLine($"Error: Student with ID {studentId} not found");
                    TempData["ErrorMessage"] = "Student account not found. Please log in again.";
                    return RedirectToAction("StudentLogin", "Auth");
                }

                Console.WriteLine($"Processing feedback submission for Student {studentId}");
                Console.WriteLine($"Feedback details: TeacherId={feedback.TeacherId}, Semester={feedback.Semester}");

                // Validate all required fields
                if (string.IsNullOrEmpty(feedback.KnowledgeInSubject) ||
                    string.IsNullOrEmpty(feedback.AbilityToExpressIdeas) ||
                    string.IsNullOrEmpty(feedback.AbilityToGiveExamples) ||
                    string.IsNullOrEmpty(feedback.AbilityToArouseInterest) ||
                    string.IsNullOrEmpty(feedback.AbilityToAttractAttention) ||
                    string.IsNullOrEmpty(feedback.GeneralBehavior) ||
                    string.IsNullOrEmpty(feedback.WillingnessToHelp) ||
                    string.IsNullOrEmpty(feedback.CommandOfEnglish) ||
                    string.IsNullOrEmpty(feedback.Punctuality) ||
                    string.IsNullOrEmpty(feedback.PreparationAndSincerity))
                {
                    Console.WriteLine("Error: Missing required feedback ratings");
                    TempData["ErrorMessage"] = "Please provide ratings for all criteria.";
                    
                    ViewBag.TeacherName = teacher.FullName;
                    ViewBag.Department = feedback.Department;
                    ViewBag.SelectedSemester = feedback.Semester;
                    
                    return View("GiveFeedback", feedback);
                }

                // Check if feedback already exists
                var existingFeedback = _context.Feedbacks
                    .FirstOrDefault(f => f.StudentId == studentId && 
                                       f.TeacherId == feedback.TeacherId && 
                                       f.Semester == feedback.Semester);

                if (existingFeedback != null)
                {
                    // Update existing feedback
                    existingFeedback.KnowledgeInSubject = feedback.KnowledgeInSubject;
                    existingFeedback.AbilityToExpressIdeas = feedback.AbilityToExpressIdeas;
                    existingFeedback.AbilityToGiveExamples = feedback.AbilityToGiveExamples;
                    existingFeedback.AbilityToArouseInterest = feedback.AbilityToArouseInterest;
                    existingFeedback.AbilityToAttractAttention = feedback.AbilityToAttractAttention;
                    existingFeedback.GeneralBehavior = feedback.GeneralBehavior;
                    existingFeedback.WillingnessToHelp = feedback.WillingnessToHelp;
                    existingFeedback.CommandOfEnglish = feedback.CommandOfEnglish;
                    existingFeedback.Punctuality = feedback.Punctuality;
                    existingFeedback.PreparationAndSincerity = feedback.PreparationAndSincerity;
                    existingFeedback.Comments = feedback.Comments;
                    existingFeedback.SubmittedAt = DateTime.Now;
                    
                    _context.Update(existingFeedback);
                }
                else
                {
                    // Set up new feedback
                    var newFeedback = new Feedback
                    {
                        StudentId = studentId,
                        TeacherId = teacher.Id, // Use verified teacher ID
                        Department = teacher.Department, // Use teacher's department
                        Semester = feedback.Semester,
                        KnowledgeInSubject = feedback.KnowledgeInSubject,
                        AbilityToExpressIdeas = feedback.AbilityToExpressIdeas,
                        AbilityToGiveExamples = feedback.AbilityToGiveExamples,
                        AbilityToArouseInterest = feedback.AbilityToArouseInterest,
                        AbilityToAttractAttention = feedback.AbilityToAttractAttention,
                        GeneralBehavior = feedback.GeneralBehavior,
                        WillingnessToHelp = feedback.WillingnessToHelp,
                        CommandOfEnglish = feedback.CommandOfEnglish,
                        Punctuality = feedback.Punctuality,
                        PreparationAndSincerity = feedback.PreparationAndSincerity,
                        Comments = feedback.Comments ?? "",
                        CreatedAt = DateTime.Now,
                        SubmittedAt = DateTime.Now
                    };

                    _context.Feedbacks.Add(newFeedback);
                }

                _context.SaveChanges();

                Console.WriteLine($"=== Feedback Successfully Saved ===");

                TempData["SuccessMessage"] = "Thank you for your valuable feedback. Your response has been recorded.";
                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== Error Saving Feedback ===");
                Console.WriteLine($"Error Type: {ex.GetType().Name}");
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                }

                var teacher = _context.Teachers.FirstOrDefault(t => t.Id == feedback.TeacherId);
                if (teacher != null)
                {
                    ViewBag.TeacherName = teacher.FullName;
                }
                ViewBag.Department = feedback.Department;
                ViewBag.SelectedSemester = feedback.Semester;

                TempData["ErrorMessage"] = $"An error occurred: {(ex.InnerException?.Message ?? ex.Message)}";
                return View("GiveFeedback", feedback);
            }
        }

        public IActionResult Success()
        {
            ViewBag.SuccessTitle = "Feedback Submitted Successfully!";
            ViewBag.SuccessMessage = TempData["SuccessMessage"] ?? "Thank you for your valuable feedback. Your response has been recorded.";
            ViewBag.ReturnUrl = Url.Action("Index", "Student");
            ViewBag.ReturnText = "Return to Dashboard";
            return View();
        }
    }
}