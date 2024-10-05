
using Library_API.Helpers;

namespace Library_API.Worker_Services
{
    public class DueDateReminderService : BackgroundService
    {
        private readonly ILogger<DueDateReminderService> _logger;
        private readonly BorrowingDueDateHelper _borrowingDueDateHelper;

        public DueDateReminderService(BorrowingDueDateHelper borrowingDueDateHelper, ILogger<DueDateReminderService> logger)
        {
            _logger = logger;
            _borrowingDueDateHelper = borrowingDueDateHelper;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
			try
			{
                while (!stoppingToken.IsCancellationRequested)
                {

                    _logger.LogInformation($"Worker service is running at: {DateTime.Now}");

                    await _borrowingDueDateHelper.DueDateReminder(stoppingToken);

                    _logger.LogInformation($"Worker service finished running at: {DateTime.Now}");

                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
			}
			catch (Exception ex)
			{
                _logger.LogError("Error in the DueDateReminder worker service: {ex}",ex.Message);
			}
        }
    }
}
