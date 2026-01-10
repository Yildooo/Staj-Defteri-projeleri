namespace LibraryManagement.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string email, string userName);
    Task SendDueDateReminderAsync(string email, string userName, string bookTitle, DateTime dueDate);
    Task SendOverdueNotificationAsync(string email, string userName, string bookTitle, int daysOverdue, decimal fineAmount);
    Task SendPasswordResetEmailAsync(string email, string resetLink);
}

