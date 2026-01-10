namespace LibraryManagement.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendWelcomeEmailAsync(string email, string userName)
    {
        // In production, integrate with an actual email service (SendGrid, etc.)
        _logger.LogInformation("Sending welcome email to {Email} for user {UserName}", email, userName);
        
        var subject = "Welcome to Library Management System";
        var body = $@"
            <h2>Welcome to Library Management System, {userName}!</h2>
            <p>Thank you for registering with our library.</p>
            <p>You can now:</p>
            <ul>
                <li>Browse our extensive book collection</li>
                <li>Borrow up to 5 books at a time</li>
                <li>View your borrowing history</li>
                <li>Receive notifications about due dates</li>
            </ul>
            <p>Happy reading!</p>
            <p>The Library Team</p>
        ";

        await SimulateSendEmailAsync(email, subject, body);
    }

    public async Task SendDueDateReminderAsync(string email, string userName, string bookTitle, DateTime dueDate)
    {
        _logger.LogInformation("Sending due date reminder to {Email} for book {BookTitle}", email, bookTitle);
        
        var subject = $"Reminder: '{bookTitle}' is due soon";
        var body = $@"
            <h2>Book Due Date Reminder</h2>
            <p>Dear {userName},</p>
            <p>This is a friendly reminder that the following book is due for return:</p>
            <p><strong>{bookTitle}</strong></p>
            <p>Due Date: {dueDate:MMMM dd, yyyy}</p>
            <p>Please return the book on time to avoid late fees.</p>
            <p>The Library Team</p>
        ";

        await SimulateSendEmailAsync(email, subject, body);
    }

    public async Task SendOverdueNotificationAsync(string email, string userName, string bookTitle, int daysOverdue, decimal fineAmount)
    {
        _logger.LogInformation("Sending overdue notification to {Email} for book {BookTitle}, {Days} days overdue", 
            email, bookTitle, daysOverdue);
        
        var subject = $"Overdue Notice: '{bookTitle}'";
        var body = $@"
            <h2>Overdue Book Notice</h2>
            <p>Dear {userName},</p>
            <p>The following book is overdue:</p>
            <p><strong>{bookTitle}</strong></p>
            <p>Days Overdue: {daysOverdue}</p>
            <p>Current Fine: ${fineAmount:F2}</p>
            <p>Please return the book as soon as possible to prevent additional fees.</p>
            <p>The Library Team</p>
        ";

        await SimulateSendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        _logger.LogInformation("Sending password reset email to {Email}", email);
        
        var subject = "Password Reset Request";
        var body = $@"
            <h2>Password Reset Request</h2>
            <p>You have requested to reset your password.</p>
            <p>Click the link below to reset your password:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            <p>If you did not request this, please ignore this email.</p>
            <p>The Library Team</p>
        ";

        await SimulateSendEmailAsync(email, subject, body);
    }

    private async Task SimulateSendEmailAsync(string to, string subject, string body)
    {
        // Simulate email sending delay
        await Task.Delay(1000);
        
        _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
        
        // In production, you would use:
        // - SendGrid: var client = new SendGridClient(apiKey);
        // - SMTP: using var smtp = new SmtpClient(host, port);
        // - Azure Communication Services
        // - AWS SES
    }
}

