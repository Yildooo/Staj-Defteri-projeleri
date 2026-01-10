using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models;

public enum BorrowStatus
{
    Active,
    Returned,
    Overdue,
    Lost
}

public class BorrowRecord
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public int BookId { get; set; }

    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public BorrowStatus Status { get; set; } = BorrowStatus.Active;

    public decimal? FineAmount { get; set; }

    public bool FinePaid { get; set; } = false;

    [StringLength(500)]
    public string? Notes { get; set; }

    public bool IsOverdue => Status == BorrowStatus.Active && DateTime.UtcNow > DueDate;

    public int DaysOverdue => IsOverdue ? (int)(DateTime.UtcNow - DueDate).TotalDays : 0;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    [ForeignKey("BookId")]
    public virtual Book? Book { get; set; }
}

