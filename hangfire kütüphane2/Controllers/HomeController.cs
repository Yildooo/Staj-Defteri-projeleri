using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LibraryManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var featuredBooks = await _context.Books
            .Where(b => b.IsActive)
            .OrderByDescending(b => b.DateAdded)
            .Take(6)
            .ToListAsync();

        ViewBag.TotalBooks = await _context.Books.CountAsync();
        ViewBag.AvailableBooks = await _context.Books.Where(b => b.AvailableCopies > 0).CountAsync();
        ViewBag.TotalMembers = await _context.Users.CountAsync();

        return View(featuredBooks);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

