using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BP.Manager.Manager
{
    public class BackgroundJobManager
    {
        private readonly IBackgroundJobStorage _jobStorage;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundJobManager(IServiceProvider serviceProvider, IBackgroundJobStorage jobStorage)
        {
            _serviceProvider = serviceProvider;
            _jobStorage = jobStorage;
        }

        public BackgroundJob Run<TTask>(Guid taskId, IBackgroundJobData data) where TTask : BackgroundJob, new()
        {
            var task = Create<TTask>(taskId)
                .Run(data);

            task.Dispose();

            return task;
        }


        public BackgroundJob Run(Guid taskId, Type taskType, IBackgroundJobData data)
        {
            BackgroundJob task =
                Create(taskId, taskType)
                .Run(data);

            task.Dispose();
            return task;
        }

        public BackgroundJob Create(Guid taskId, Type taskType)
        {
            BackgroundJob instance = (BackgroundJob)Activator.CreateInstance(taskType);

            var task = instance
                .Configure(taskId, _serviceProvider.CreateScope())
                .onStart((task) => _jobStorage.Add(task))
                .onComplete((task) => _jobStorage.TryRemove(task.Id, out var removedTask));
            return task;
        }

        public BackgroundJob Create<TTask>(Guid taskId) where TTask : BackgroundJob, new()
        {
            var task = new TTask()
                .Configure(taskId, _serviceProvider.CreateScope())
                .onStart((task) => _jobStorage.Add(task))
                .onComplete((task) => _jobStorage.TryRemove(task.Id, out var removedTask));

            return task;
        }

        public void Cancel(Guid id)
        {
            if (_jobStorage.TryRemove(id, out var task))
            {
                task.Stop();
            }
        }

        public void Remove(Guid id)
        {
            if (_jobStorage.TryRemove(id, out var task))
            {
                task.Dispose();
            }
        }

        public IReadOnlyCollection<BackgroundJob> Get()
        {
            return _jobStorage.Get();
        }

        public void RestartTasks()
        {
            var notFinishedTasks = GetNotFinishedTasks();
            if (notFinishedTasks.Any())
            {
                foreach (var notFinishedTask in notFinishedTasks)
                {
                    Restart(notFinishedTask);
                }
            }
        }

        private void Restart(BackgroundJob notFinishedTask)
        {
            Run(notFinishedTask.Id, notFinishedTask.GetType(), notFinishedTask.Data) ;
        }

        private IEnumerable<BackgroundJob> GetNotFinishedTasks()
        {
            var tasks = _jobStorage.Get();
            return tasks.Where(t => t.Status == Domain.Enums.BackgroundJobstatus.Started);
        }

    }
}
