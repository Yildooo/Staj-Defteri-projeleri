using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string ISBN { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Publisher { get; set; }

    public int? PublishedYear { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(500)]
    public string? CoverImageUrl { get; set; }

    public int TotalCopies { get; set; } = 1;

    public int AvailableCopies { get; set; } = 1;

    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public bool IsAvailable => AvailableCopies > 0;

    // Navigation properties
    public virtual ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}

