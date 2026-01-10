using LibraryManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if we already have books
        if (await context.Books.AnyAsync())
        {
            return; // DB has been seeded
        }

        // Seed Books
        var books = new Book[]
        {
            new Book
            {
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                ISBN = "978-0743273565",
                Description = "A story of decadence and excess, Gatsby explores the American Dream.",
                Publisher = "Scribner",
                PublishedYear = 1925,
                Category = "Fiction",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780743273565-L.jpg",
                TotalCopies = 5,
                AvailableCopies = 5
            },
            new Book
            {
                Title = "To Kill a Mockingbird",
                Author = "Harper Lee",
                ISBN = "978-0061120084",
                Description = "A classic of modern American literature about racial injustice.",
                Publisher = "Harper Perennial",
                PublishedYear = 1960,
                Category = "Fiction",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780061120084-L.jpg",
                TotalCopies = 4,
                AvailableCopies = 4
            },
            new Book
            {
                Title = "1984",
                Author = "George Orwell",
                ISBN = "978-0451524935",
                Description = "A dystopian social science fiction novel and cautionary tale.",
                Publisher = "Signet Classic",
                PublishedYear = 1949,
                Category = "Science Fiction",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780451524935-L.jpg",
                TotalCopies = 3,
                AvailableCopies = 3
            },
            new Book
            {
                Title = "Pride and Prejudice",
                Author = "Jane Austen",
                ISBN = "978-0141439518",
                Description = "A romantic novel following the emotional development of Elizabeth Bennet.",
                Publisher = "Penguin Classics",
                PublishedYear = 1813,
                Category = "Romance",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780141439518-L.jpg",
                TotalCopies = 4,
                AvailableCopies = 4
            },
            new Book
            {
                Title = "The Catcher in the Rye",
                Author = "J.D. Salinger",
                ISBN = "978-0316769488",
                Description = "A story about teenage alienation and loss of innocence.",
                Publisher = "Little, Brown and Company",
                PublishedYear = 1951,
                Category = "Fiction",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780316769488-L.jpg",
                TotalCopies = 3,
                AvailableCopies = 3
            },
            new Book
            {
                Title = "The Hobbit",
                Author = "J.R.R. Tolkien",
                ISBN = "978-0547928227",
                Description = "A fantasy novel about the adventures of hobbit Bilbo Baggins.",
                Publisher = "Houghton Mifflin Harcourt",
                PublishedYear = 1937,
                Category = "Fiction",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780547928227-L.jpg",
                TotalCopies = 4,
                AvailableCopies = 4
            },
            new Book
            {
                Title = "Harry Potter and the Sorcerer's Stone",
                Author = "J.K. Rowling",
                ISBN = "978-0590353427",
                Description = "The first book in the Harry Potter series about a young wizard.",
                Publisher = "Scholastic",
                PublishedYear = 1997,
                Category = "Fiction",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780590353427-L.jpg",
                TotalCopies = 6,
                AvailableCopies = 6
            },
            new Book
            {
                Title = "The Lord of the Rings",
                Author = "J.R.R. Tolkien",
                ISBN = "978-0618640157",
                Description = "An epic high-fantasy novel set in Middle-earth.",
                Publisher = "Mariner Books",
                PublishedYear = 1954,
                Category = "Fiction",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780618640157-L.jpg",
                TotalCopies = 3,
                AvailableCopies = 3
            },
            new Book
            {
                Title = "Brave New World",
                Author = "Aldous Huxley",
                ISBN = "978-0060850524",
                Description = "A dystopian novel set in a futuristic World State.",
                Publisher = "Harper Perennial",
                PublishedYear = 1932,
                Category = "Science Fiction",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780060850524-L.jpg",
                TotalCopies = 3,
                AvailableCopies = 3
            },
            new Book
            {
                Title = "The Alchemist",
                Author = "Paulo Coelho",
                ISBN = "978-0062315007",
                Description = "A philosophical story about following your dreams.",
                Publisher = "HarperOne",
                PublishedYear = 1988,
                Category = "Fiction",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780062315007-L.jpg",
                TotalCopies = 4,
                AvailableCopies = 4
            }
        };

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();

        // Create admin role and user
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "Member" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create admin user
        var adminEmail = "admin@library.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

