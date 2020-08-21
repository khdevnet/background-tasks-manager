using BP.Manager.Domain.Entity;
using BP.Manager.Domain.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace BP.Manager.Manager
{
    public class BackgroundTaskSessionManager
    {
        private readonly BackgroundTaskManager _manager;
        private readonly BackgroundTaskRepository _repository;

        public BackgroundTaskSessionManager(BackgroundTaskManager manager, BackgroundTaskRepository repository)
        {
            _manager = manager;
            _repository = repository;
        }

        public async Task<Guid> StartAsync(IBackgroundTaskData taskData, string userName)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var taskId = Guid.NewGuid();
                var id = _manager.Start(taskId, taskData, CompleteAsync);
                await _repository.Create(new BackgroundTaskEntity
                {
                    Id = id,
                    Status = Domain.Enums.BackgroundTaskStatus.Started,
                    TaskType = taskData.GetType().FullName,
                    UserName = userName,
                    Data = JsonConvert.SerializeObject(taskData)
                });
                scope.Complete();
                return id;
            }
        }

        public async Task RestartAsync(Guid id)
        {
            var notFinishedTask = await GetNotFinishedTask(id);
            var data = JsonConvert.DeserializeObject(notFinishedTask.Data, Type.GetType(notFinishedTask.TaskType));
            _manager.Start(notFinishedTask.Id, (IBackgroundTaskData)data, CompleteAsync);
        }

        public async Task CompleteAsync(Guid id)
        {
            await _repository.SetStatus(id, Domain.Enums.BackgroundTaskStatus.Finished);
        }

        public async Task CancelAsync(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _repository.SetStatus(id, Domain.Enums.BackgroundTaskStatus.Cancel);
                _manager.Cancel(id);
                scope.Complete();
            }
        }

        public async Task Remove(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _repository.Remove(id);
                _manager.Remove(id);
                scope.Complete();
            }
        }

        public async Task<IEnumerable<BackgroundTaskEntity>> Get()
        {
            return await _repository.Get();
        }

        private async Task<BackgroundTaskEntity> GetNotFinishedTask(Guid id)
        {
            var tasks = await _repository.Get();
            return tasks.FirstOrDefault(t => t.Id == id && t.Status == Domain.Enums.BackgroundTaskStatus.Started);
        }
    }
}
