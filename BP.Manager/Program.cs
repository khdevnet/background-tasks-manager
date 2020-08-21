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
                      services.AddSingleton<BackgroundProcessManager>();
                      services.AddScoped<IBackgroundTaskHandler<MonitorBackgroundTask>, MonitorBackgroundTaskHandler>();
                  })
                  .Build();

            await host.StartAsync();

            var manager = host.Services.GetService<BackgroundProcessManager>();

            manager.Start(new MonitorBackgroundTask());
            manager.Start(new MonitorBackgroundTask());
            manager.Start(new MonitorBackgroundTask());

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
