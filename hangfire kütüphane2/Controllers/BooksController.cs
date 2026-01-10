using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers;

public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BooksController> _logger;

    public BooksController(ApplicationDbContext context, ILogger<BooksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Books
    public async Task<IActionResult> Index(string? searchString, string? category, string? sortOrder)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentCategory"] = category;
        ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
        ViewData["AuthorSortParm"] = sortOrder == "author" ? "author_desc" : "author";
        ViewData["CurrentSort"] = sortOrder;

        var books = _context.Books.Where(b => b.IsActive);

        // Search
        if (!String.IsNullOrEmpty(searchString))
        {
            books = books.Where(b => 
                b.Title.Contains(searchString) || 
                b.Author.Contains(searchString) ||
                b.ISBN.Contains(searchString));
        }

        // Category filter
        if (!String.IsNullOrEmpty(category))
        {
            books = books.Where(b => b.Category == category);
        }

        // Sorting
        books = sortOrder switch
        {
            "title_desc" => books.OrderByDescending(b => b.Title),
            "author" => books.OrderBy(b => b.Author),
            "author_desc" => books.OrderByDescending(b => b.Author),
            _ => books.OrderBy(b => b.Title),
        };

        // Get categories for filter dropdown
        ViewBag.Categories = await _context.Books
            .Where(b => b.Category != null)
            .Select(b => b.Category)
            .Distinct()
            .ToListAsync();

        return View(await books.ToListAsync());
    }

    // GET: Books/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: Books/Create (Admin only)
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(BookViewModel model)
    {
        if (ModelState.IsValid)
        {
            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                ISBN = model.ISBN,
                Description = model.Description,
                Publisher = model.Publisher,
                PublishedYear = model.PublishedYear,
                Category = model.Category,
                CoverImageUrl = model.CoverImageUrl,
                TotalCopies = model.TotalCopies,
                AvailableCopies = model.TotalCopies
            };

            _context.Add(book);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Book added successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Books/Edit/5 (Admin only)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        var model = new BookViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            Description = book.Description,
            Publisher = book.Publisher,
            PublishedYear = book.PublishedYear,
            Category = book.Category,
            CoverImageUrl = book.CoverImageUrl,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies
        };

        return View(model);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, BookViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            book.Title = model.Title;
            book.Author = model.Author;
            book.ISBN = model.ISBN;
            book.Description = model.Description;
            book.Publisher = model.Publisher;
            book.PublishedYear = model.PublishedYear;
            book.Category = model.Category;
            book.CoverImageUrl = model.CoverImageUrl;
            book.TotalCopies = model.TotalCopies;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Book updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }
}

