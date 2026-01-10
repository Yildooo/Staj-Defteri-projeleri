using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services;

public class LibraryJobService : ILibraryJobService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<LibraryJobService> _logger;

    public LibraryJobService(
        ApplicationDbContext context,
        IEmailService emailService,
        ILogger<LibraryJobService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SendOverdueReminders()
    {
        _logger.LogInformation("Starting overdue reminders job at {Time}", DateTime.UtcNow);

        var overdueRecords = await _context.BorrowRecords
            .Include(br => br.User)
            .Include(br => br.Book)
            .Where(br => br.Status == BorrowStatus.Active && br.DueDate < DateTime.UtcNow)
            .ToListAsync();

        foreach (var record in overdueRecords)
        {
            if (record.User != null && record.Book != null)
            {
                record.Status = BorrowStatus.Overdue;
                
                var daysOverdue = (int)(DateTime.UtcNow - record.DueDate).TotalDays;
                var fineAmount = daysOverdue * 0.50m;
                record.FineAmount = fineAmount;

                await _emailService.SendOverdueNotificationAsync(
                    record.User.Email!,
                    record.User.FullName,
                    record.Book.Title,
                    daysOverdue,
                    fineAmount);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Processed {Count} overdue records", overdueRecords.Count);
    }

    public async Task CleanupOldRecords()
    {
        _logger.LogInformation("Starting database cleanup job at {Time}", DateTime.UtcNow);

        // Archive records older than 2 years
        var cutoffDate = DateTime.UtcNow.AddYears(-2);
        
        var oldRecords = await _context.BorrowRecords
            .Where(br => br.Status == BorrowStatus.Returned && br.ReturnDate < cutoffDate)
            .ToListAsync();

        // In a real application, you might archive these to a separate table
        // For now, we'll just log the count
        _logger.LogInformation("Found {Count} records older than 2 years eligible for archival", oldRecords.Count);

        // Remove inactive books with no borrow history
        var inactiveBooks = await _context.Books
            .Where(b => !b.IsActive && !b.BorrowRecords.Any())
            .ToListAsync();

        if (inactiveBooks.Any())
        {
            _context.Books.RemoveRange(inactiveBooks);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Removed {Count} inactive books with no history", inactiveBooks.Count);
        }
    }

    public async Task UpdateBookAvailability()
    {
        _logger.LogInformation("Starting book availability update job at {Time}", DateTime.UtcNow);

        var books = await _context.Books.Include(b => b.BorrowRecords).ToListAsync();

        foreach (var book in books)
        {
            var activeBorrows = book.BorrowRecords.Count(br => 
                br.Status == BorrowStatus.Active || br.Status == BorrowStatus.Overdue);
            
            var expectedAvailable = book.TotalCopies - activeBorrows;
            
            if (book.AvailableCopies != expectedAvailable)
            {
                _logger.LogWarning("Book {BookId} availability mismatch. Expected: {Expected}, Actual: {Actual}",
                    book.Id, expectedAvailable, book.AvailableCopies);
                book.AvailableCopies = expectedAvailable;
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Book availability update completed");
    }

    public async Task ProcessPendingFines()
    {
        _logger.LogInformation("Starting fine processing job at {Time}", DateTime.UtcNow);

        var overdueRecords = await _context.BorrowRecords
            .Where(br => br.Status == BorrowStatus.Overdue && !br.FinePaid)
            .ToListAsync();

        foreach (var record in overdueRecords)
        {
            var daysOverdue = (int)(DateTime.UtcNow - record.DueDate).TotalDays;
            record.FineAmount = daysOverdue * 0.50m; // $0.50 per day
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated fines for {Count} overdue records", overdueRecords.Count);
    }
}

