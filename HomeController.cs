using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FeedbackSystem.Models;

namespace FeedbackSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FeedbackContext _context;

    public HomeController(ILogger<HomeController> logger, FeedbackContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        // We don't need to initialize required properties since we're only using it for the form model
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
