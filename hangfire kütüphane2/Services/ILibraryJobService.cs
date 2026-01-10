namespace LibraryManagement.Services;

public interface ILibraryJobService
{
    Task SendOverdueReminders();
    Task CleanupOldRecords();
    Task UpdateBookAvailability();
    Task ProcessPendingFines();
}

