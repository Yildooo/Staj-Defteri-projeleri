using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hangfire;

namespace LibraryManagement.Controllers;

[Authorize]
public class BorrowController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BorrowController> _logger;

    public BorrowController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<BorrowController> logger)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    // POST: Borrow/BorrowBook/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BorrowBook(int bookId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
        {
            TempData["ErrorMessage"] = "Book not found.";
            return RedirectToAction("Index", "Books");
        }

        if (book.AvailableCopies <= 0)
        {
            TempData["ErrorMessage"] = "This book is currently not available.";
            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        // Check if user already has this book borrowed
        var existingBorrow = await _context.BorrowRecords
            .AnyAsync(br => br.UserId == user.Id && br.BookId == bookId && br.Status == BorrowStatus.Active);

        if (existingBorrow)
        {
            TempData["ErrorMessage"] = "You already have this book borrowed.";
            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        // Check max books limit
        var maxBooks = _configuration.GetValue<int>("LibrarySettings:MaxBooksPerUser", 5);
        var currentBorrows = await _context.BorrowRecords
            .CountAsync(br => br.UserId == user.Id && br.Status == BorrowStatus.Active);

        if (currentBorrows >= maxBooks)
        {
            TempData["ErrorMessage"] = $"You have reached the maximum limit of {maxBooks} books.";
            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        // Create borrow record
        var loanDays = _configuration.GetValue<int>("LibrarySettings:DefaultLoanDays", 14);
        var borrowRecord = new BorrowRecord
        {
            UserId = user.Id,
            BookId = bookId,
            BorrowDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(loanDays),
            Status = BorrowStatus.Active
        };

        book.AvailableCopies--;
        _context.BorrowRecords.Add(borrowRecord);
        await _context.SaveChangesAsync();

        // Schedule reminder email 2 days before due date
        var reminderDate = borrowRecord.DueDate.AddDays(-2);
        if (reminderDate > DateTime.UtcNow && !string.IsNullOrEmpty(user.Email))
        {
            BackgroundJob.Schedule<IEmailService>(
                x => x.SendDueDateReminderAsync(user.Email!, user.FullName, book.Title, borrowRecord.DueDate),
                reminderDate);
        }

        TempData["SuccessMessage"] = $"You have successfully borrowed '{book.Title}'. Due date: {borrowRecord.DueDate:MMM dd, yyyy}";
        _logger.LogInformation("User {UserId} borrowed book {BookId}", user.Id, bookId);

        return RedirectToAction("MyBooks", "Dashboard");
    }

    // POST: Borrow/ReturnBook/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReturnBook(int borrowId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var borrowRecord = await _context.BorrowRecords
            .Include(br => br.Book)
            .FirstOrDefaultAsync(br => br.Id == borrowId && br.UserId == user.Id);

        if (borrowRecord == null)
        {
            TempData["ErrorMessage"] = "Borrow record not found.";
            return RedirectToAction("MyBooks", "Dashboard");
        }

        if (borrowRecord.Status != BorrowStatus.Active && borrowRecord.Status != BorrowStatus.Overdue)
        {
            TempData["ErrorMessage"] = "This book has already been returned.";
            return RedirectToAction("MyBooks", "Dashboard");
        }

        // Calculate fine if overdue
        if (borrowRecord.DueDate < DateTime.UtcNow)
        {
            var daysOverdue = (int)(DateTime.UtcNow - borrowRecord.DueDate).TotalDays;
            borrowRecord.FineAmount = daysOverdue * 0.50m; // $0.50 per day
        }

        borrowRecord.ReturnDate = DateTime.UtcNow;
        borrowRecord.Status = BorrowStatus.Returned;

        if (borrowRecord.Book != null)
        {
            borrowRecord.Book.AvailableCopies++;
        }

        await _context.SaveChangesAsync();

        var message = $"You have successfully returned '{borrowRecord.Book?.Title}'.";
        if (borrowRecord.FineAmount > 0)
        {
            message += $" Fine amount: ${borrowRecord.FineAmount:F2}";
        }

        TempData["SuccessMessage"] = message;
        _logger.LogInformation("User {UserId} returned book {BookId}", user.Id, borrowRecord.BookId);

        return RedirectToAction("MyBooks", "Dashboard");
    }
}

