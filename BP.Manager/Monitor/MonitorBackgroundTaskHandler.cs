using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BP.Manager.Manager
{


    public class MonitorBackgroundTaskHandler : IBackgroundTaskHandler<MonitorBackgroundTask>
    {
        private readonly ILogger<MonitorBackgroundTaskHandler> _logger;
        private readonly BackgroundProcessManager _manager;

        public MonitorBackgroundTaskHandler(ILogger<MonitorBackgroundTaskHandler> logger, BackgroundProcessManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        public async Task Start(BackgroundProcess bt, MonitorBackgroundTask data)
        {
            // Simulate three 5-second tasks to complete
            // for each enqueued work item


            int delayLoop = 0;
            var guid = bt.Id.ToString();

            _logger.LogInformation(
                "Queued Background Task {Guid} is starting.", guid);

            while (!bt.Token.IsCancellationRequested && delayLoop < 3)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(20), bt.Token);
                }
                catch (OperationCanceledException)
                {
                    // Prevent throwing if the Delay is cancelled
                }

                delayLoop++;

                _logger.LogInformation(
                    "Queued Background Task {Guid} is running. " +
                    "{DelayLoop}/3", guid, delayLoop);
            }

            if (delayLoop == 3)
            {
                _logger.LogInformation(
                    "Queued Background Task {Guid} is complete.", guid);

            }
            else
            {
                _logger.LogInformation(
                    "Queued Background Task {Guid} was cancelled.", guid);
            }

            _logger.LogInformation("Queued Background Task Left {Guid}.", _manager.Get().Count);
        }
    }
}
