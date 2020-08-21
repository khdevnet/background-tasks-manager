using BP.Manager.Manager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BP.Manager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                  .ConfigureServices((hostContext, services) =>
                  {
                      services.AddSingleton<BackgroundTaskManager>();
                      services.AddScoped<IBackgroundTaskHandler<MonitorBackgroundTaskData>, MonitorBackgroundTaskHandler>();
                  })
                  .Build();

            await host.StartAsync();

            var manager = host.Services.GetService<BackgroundTaskManager>();

            manager.Start(new MonitorBackgroundTaskData());
            manager.Start(new MonitorBackgroundTaskData());
            manager.Start(new MonitorBackgroundTaskData());

            Console.WriteLine("Count: " + manager.Get().Count);

            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                Console.WriteLine("Cancel task by token id: ");
                var taskId = Console.ReadLine();
                manager.CancelTask(Guid.Parse(taskId));
            }

            await host.WaitForShutdownAsync();

        }
    }
}
