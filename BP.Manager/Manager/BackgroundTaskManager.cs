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

        public Guid Start<TBackgroundTask>(TBackgroundTask data) where TBackgroundTask: struct, IBackgroundTaskData
        {
            var taskId = Guid.NewGuid();
            Task.Run(async () =>
            {
                using (var task = new BackgroundTask(taskId, _serviceProvider))
                {
                    tasks.TryAdd(task.Id, task);
                    await task.Start<TBackgroundTask>(data);
                    tasks.TryRemove(task.Id, out var removedTask);
                }
            });
            return taskId;
        }

        public void CancelTask(Guid id)
        {
            if (tasks.TryRemove(id, out var task))
            {
                task.Cancel();
            }
        }

        public IReadOnlyCollection<BackgroundTask> Get()
        {
            return new ReadOnlyCollection<BackgroundTask>(tasks.Values.ToList());
        }

    }
}
