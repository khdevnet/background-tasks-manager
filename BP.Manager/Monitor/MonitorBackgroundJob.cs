using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BP.Manager.Manager
{
    public class MonitorBackgroundJob : BackgroundJobBase<MonitorJobData>
    {
        protected override async Task Run(MonitorJobData data)
        {
            // Simulate three 5-second tasks to complete
            //            // for each enqueued work item

            var _logger = GetService<ILogger<MonitorBackgroundJob>>();
            var _manager = GetService<BackgroundJobSessionManager>();

            int delayLoop = 0;
            var guid = Id.ToString();

            _logger.LogInformation(
                "Queued Background Task {Guid} is starting.", guid);

            while (!Token.IsCancellationRequested && delayLoop < 3)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), Token);
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

            _logger.LogInformation("Queued Background Task Left {Guid}.", _manager.Get().Count());
        }
    }
}
