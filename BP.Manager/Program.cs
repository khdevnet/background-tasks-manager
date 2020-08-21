using BP.Manager.Domain.Database;
using BP.Manager.Domain.Entity;
using BP.Manager.Domain.Repositories;
using BP.Manager.Manager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
                      services.AddSingleton<BackgroundTaskRepository>();
                      services.AddSingleton<BackgroundTaskManager>();
                      services.AddSingleton<BackgroundTaskSessionManager>();
                      services.AddScoped<IBackgroundTaskHandler<MonitorBackgroundTaskData>, MonitorBackgroundTaskHandler>();
                      services.AddDbContext<TaskStatesContext>();
                  })
                  .Build();

            await host.StartAsync();

            DatabaseMigrator.Migrate(host.Services);

            var rep = host.Services.GetService<BackgroundTaskRepository>();
            var manager = host.Services.GetService<BackgroundTaskSessionManager>();

            await RestartTasks(manager, rep);

            await CreateTask(rep, manager);
            //await CreateTask(rep, manager);
            //await CreateTask(rep, manager);

            Console.WriteLine("Count: " + (await manager.Get()).Where(x => x.Status == Domain.Enums.BackgroundTaskStatus.Started).Count());

            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                Console.WriteLine("Cancel task by token id: ");
                var taskId = Console.ReadLine();
                await manager.CancelAsync(Guid.Parse(taskId));
            }

            await host.WaitForShutdownAsync();

        }

        private static async Task CreateTask(BackgroundTaskRepository rep, BackgroundTaskSessionManager manager)
        {
            var taskData = new MonitorBackgroundTaskData();
            var id = await manager.StartAsync(taskData, "Anton");
        }

        private static async Task RestartTasks(BackgroundTaskSessionManager manager, BackgroundTaskRepository rep)
        {
            var notFinishedTasks = await GetNotFinishedTasks(rep);
            if (notFinishedTasks.Any())
            {
                foreach (var notFinishedTask in notFinishedTasks)
                {
                    await manager.RestartAsync(notFinishedTask.Id);
                }
            }
        }

        private static async Task<IEnumerable<BackgroundTaskEntity>> GetNotFinishedTasks(BackgroundTaskRepository rep)
        {
            var tasks = await rep.Get();
            return tasks.Where(t => t.Status == Domain.Enums.BackgroundTaskStatus.Started);
        }
    }
}
