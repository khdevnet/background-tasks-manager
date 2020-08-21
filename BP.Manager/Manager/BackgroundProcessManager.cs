using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BP.Manager.Manager
{
    public class BackgroundProcessManager
    {
        private readonly ConcurrentDictionary<Guid, BackgroundProcess> tasks = new ConcurrentDictionary<Guid, BackgroundProcess>();
        private readonly IServiceProvider _serviceProvider;

        public BackgroundProcessManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Guid Start<TBackgroundTask>(TBackgroundTask data) where TBackgroundTask: struct, IBackgroundTask
        {
            var taskId = Guid.NewGuid();
            Task.Run(async () =>
            {
                using (var task = new BackgroundProcess(taskId, _serviceProvider))
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

        public IReadOnlyCollection<BackgroundProcess> Get()
        {
            return new ReadOnlyCollection<BackgroundProcess>(tasks.Values.ToList());
        }

    }
}
