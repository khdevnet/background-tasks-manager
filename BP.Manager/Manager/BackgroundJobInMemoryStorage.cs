using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BP.Manager.Manager
{
    public class BackgroundJobInMemoryStorage : IBackgroundJobStorage
    {
        private readonly ConcurrentDictionary<Guid, BackgroundJob> tasks = new ConcurrentDictionary<Guid, BackgroundJob>();

        public void Add(BackgroundJob task)
        {
            tasks.TryAdd(task.Id, task);
        }

        public bool TryRemove(Guid taskId, out BackgroundJob removedTask)
        {
            var isRemoved = tasks.TryRemove(taskId, out var task);
            removedTask = task;
            return isRemoved;
        }

        public IReadOnlyCollection<BackgroundJob> Get()
        {
            return new ReadOnlyCollection<BackgroundJob>(tasks.Values.ToList());
        }

    }
}
