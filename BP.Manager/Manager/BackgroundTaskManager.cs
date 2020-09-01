using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BP.Manager.Manager
{
    public class BackgroundTaskManager
    {
        private readonly ConcurrentDictionary<Guid, BackgroundTask> tasks = new ConcurrentDictionary<Guid, BackgroundTask>();
        private readonly IServiceProvider _serviceProvider;

        public BackgroundTaskManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Guid Start(Guid taskId, IBackgroundTaskData data, Func<Guid, Task> complete = null)
        {
            Task.Run(async () =>
            {
                using (var task = new BackgroundTask(taskId, _serviceProvider.CreateScope()))
                {
                    tasks.TryAdd(task.Id, task);
                    await task.Start(data);
                    tasks.TryRemove(task.Id, out var removedTask);
                    await complete?.Invoke(removedTask.Id);
                }
            });
            return taskId;
        }

        public void Cancel(Guid id)
        {
            if (tasks.TryRemove(id, out var task))
            {
                task.Cancel();
            }
        }

        public void Remove(Guid id)
        {
            if (tasks.TryRemove(id, out var task))
            {
                task.Dispose();
            }
        }

        public IReadOnlyCollection<BackgroundTask> Get()
        {
            return new ReadOnlyCollection<BackgroundTask>(tasks.Values.ToList());
        }

    }
}
