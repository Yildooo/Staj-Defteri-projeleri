using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var currentBorrows = await _context.BorrowRecords
            .Include(br => br.Book)
            .Where(br => br.UserId == user.Id && br.Status == BorrowStatus.Active)
            .ToListAsync();

        var borrowHistory = await _context.BorrowRecords
            .Include(br => br.Book)
            .Where(br => br.UserId == user.Id && br.Status == BorrowStatus.Returned)
            .OrderByDescending(br => br.ReturnDate)
            .Take(10)
            .ToListAsync();

        var viewModel = new UserDashboardViewModel
        {
            User = user,
            CurrentBorrows = currentBorrows,
            BorrowHistory = borrowHistory,
            TotalBooksBorrowed = await _context.BorrowRecords.CountAsync(br => br.UserId == user.Id),
            OverdueBooks = currentBorrows.Count(br => br.IsOverdue),
            TotalFines = await _context.BorrowRecords
                .Where(br => br.UserId == user.Id && br.FineAmount > 0 && !br.FinePaid)
                .SumAsync(br => br.FineAmount ?? 0)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> MyBooks()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var currentBorrows = await _context.BorrowRecords
            .Include(br => br.Book)
            .Where(br => br.UserId == user.Id && 
                   (br.Status == BorrowStatus.Active || br.Status == BorrowStatus.Overdue))
            .OrderBy(br => br.DueDate)
            .ToListAsync();

        // Update overdue status
        foreach (var borrow in currentBorrows.Where(b => b.IsOverdue && b.Status == BorrowStatus.Active))
        {
            borrow.Status = BorrowStatus.Overdue;
        }
        await _context.SaveChangesAsync();

        return View(currentBorrows);
    }

    public async Task<IActionResult> History()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var history = await _context.BorrowRecords
            .Include(br => br.Book)
            .Where(br => br.UserId == user.Id)
            .OrderByDescending(br => br.BorrowDate)
            .ToListAsync();

        return View(history);
    }

    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string firstName, string lastName, string? address)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Address = address;

        await _userManager.UpdateAsync(user);
        TempData["SuccessMessage"] = "Profile updated successfully!";

        return RedirectToAction(nameof(Profile));
    }
}

