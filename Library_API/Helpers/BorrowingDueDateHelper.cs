using Library_API.Repositories;
using Library_API.Services;

namespace Library_API.Helpers
{
    public class BorrowingDueDateHelper
    {
        private readonly IBorrowing _borrowingRepo;
        private readonly EmailService _emailService;
        private readonly ILogger<BorrowingDueDateHelper> _logger;

        public BorrowingDueDateHelper(IBorrowing borrowingRpo, EmailService emailService, ILogger<BorrowingDueDateHelper> logger)
        {
            _borrowingRepo = borrowingRpo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task DueDateReminder(CancellationToken stoppingToken)
        {
            try
            {
                var borrowings = _borrowingRepo.GetCurrentBorrowings();

                if (borrowings.Count() > 0)
                {
                    foreach (var borrowing in borrowings)
                    {
                        var days = Math.Floor((borrowing.DueDate - DateTime.Today).TotalDays);

                        if (days == 1)
                        {
                            string email = borrowing.Email;
                            string subject = "Due Date Reminder";
                            string body = $"This book: {borrowing.Title} you borrowed in: {borrowing.BorrowDate} is due tomorrow" +
                                $", please do not bringit later than tomorrow or there will be a fine";

                            await _emailService.SendEmail(email, subject, body);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Due date reminder helper: {ex}", ex.Message);
            }
        }

    }
}
