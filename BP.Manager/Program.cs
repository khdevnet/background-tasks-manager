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
                      services.AddSingleton<BackgroundJobRepository>();
                      services.AddSingleton<IBackgroundJobStorage, BackgroundJobInMemoryStorage>();
                      services.AddSingleton<BackgroundJobManager>();
                      services.AddSingleton<BackgroundJobSessionManager>();
                      services.AddDbContext<BackgroundJobsContext>();
                  })
                  .Build();

            await host.StartAsync();

            DatabaseMigrator.Migrate(host.Services);

            var rep = host.Services.GetService<BackgroundJobRepository>();
            var manager = host.Services.GetService<BackgroundJobSessionManager>();

            manager.RestartTasks();

            CreateTask(manager);
            //await CreateTask(rep, manager);
            //await CreateTask(rep, manager);

            Console.WriteLine("Count: " + (manager.Get()).Where(x => x.Status == Domain.Enums.BackgroundJobstatus.Started).Count());

            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                Console.WriteLine("Cancel task by token id: ");
                var taskId = Console.ReadLine();
                manager.Cancel(Guid.Parse(taskId));
            }

            await host.WaitForShutdownAsync();

        }

        private static void CreateTask(BackgroundJobSessionManager manager)
        {
            var taskData = new MonitorJobData();
            var id = manager.RunAsync<MonitorBackgroundJob>(taskData, "Anton");
        }


    }
}
