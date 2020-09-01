using BP.Manager.Domain.Entity;
using BP.Manager.Domain.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BP.Manager.Manager
{
    public class BackgroundJobSessionManager
    {
        private readonly BackgroundJobManager _manager;
        private readonly BackgroundJobRepository _repository;

        public BackgroundJobSessionManager(BackgroundJobManager manager, BackgroundJobRepository repository)
        {
            _manager = manager;
            _repository = repository;
        }

        public Guid RunAsync<TTask>(IBackgroundJobData taskData, string userName) where TTask : BackgroundJob, new()
        {
            var taskId = Guid.NewGuid();

            var task = _manager.Create<TTask>(taskId)
                  .onStart((task) => Start(taskData, userName, task))
                  .onComplete((task) => Complete(task.Id))
                  .Run(taskData);

            return taskId;
        }

        private void Start(IBackgroundJobData taskData, string userName, BackgroundJob bt)
        {
            _repository.Create(new BackgroundJobState
            {
                Id = bt.Id,
                Status = Domain.Enums.BackgroundJobstatus.Started,
                TaskDataType = taskData.GetType().FullName,
                TaskType = bt.GetType().FullName,
                GroupName = userName,
                Data = JsonConvert.SerializeObject(taskData)
            });
        }

        public void Complete(Guid id)
        {
            _repository.SetStatus(id, Domain.Enums.BackgroundJobstatus.Finished);
        }

        public void Cancel(Guid id)
        {
            _repository.SetStatus(id, Domain.Enums.BackgroundJobstatus.Cancel);
            _manager.Cancel(id);
        }

        public void Remove(Guid id)
        {
            _repository.Remove(id);
            _manager.Remove(id);
        }

        public void RestartTasks()
        {
            var notFinishedTasks =  GetNotFinishedTasks();
            if (notFinishedTasks.Any())
            {
                foreach (var notFinishedTask in notFinishedTasks)
                {
                    Restart(notFinishedTask);
                }
            }
        }

        public IEnumerable<BackgroundJobState> Get()
        {
            return _repository.Get();
        }

        private void Restart(BackgroundJobState notFinishedTask)
        {
            var taskData = (IBackgroundJobData)JsonConvert.DeserializeObject(notFinishedTask.Data, Type.GetType(notFinishedTask.TaskDataType));
            _manager.Create(notFinishedTask.Id, Type.GetType(notFinishedTask.TaskType))
                .onComplete((task) => Complete(task.Id))
                .Run(taskData);
        }

        private IEnumerable<BackgroundJobState> GetNotFinishedTasks()
        {
            var tasks = _repository.Get();
            return tasks.Where(t => t.Status == Domain.Enums.BackgroundJobstatus.Started);
        }
    }
}
