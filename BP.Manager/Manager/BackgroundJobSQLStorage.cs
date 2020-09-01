//using BP.Manager.Domain.Entity;
//using BP.Manager.Domain.Repositories;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace BP.Manager.Manager
//{
//    public class BackgroundJobSQLStorage : IBackgroundJobSQLStorage
//    {
//        private readonly IBackgroundJobStorage _jobStorage;
//        private readonly BackgroundJobRepository _repository;

//        public BackgroundJobSQLStorage(IBackgroundJobStorage jobStorage, BackgroundJobRepository repository)
//        {
//            _jobStorage = jobStorage;
//            _repository = repository;
//        }

//        public void Add(BackgroundJob task)
//        {
//            _jobStorage.Add(task);
//            Create("anton", task);
//        }

//        public bool TryRemove(Guid taskId, out BackgroundJob removedTask)
//        {
//            _repository.Remove(taskId);
//            var isRemoved = _jobStorage.TryRemove(taskId, out var task);
//            removedTask = task;

//            return isRemoved;
//        }

//        public IReadOnlyCollection<BackgroundJob> Get()
//        {
//            return _jobStorage.Get();
//        }

//        private void Create(string userName, BackgroundJob bt)
//        {
//            _repository.Create(new Domain.Entity.BackgroundJobState
//            {
//                Id = bt.Id,
//                Status = Domain.Enums.BackgroundJobstatus.Started,
//                TaskDataType = bt.Data.GetType().FullName,
//                TaskType = bt.GetType().FullName,
//                GroupName = userName,
//                Data = JsonConvert.SerializeObject(bt.Data)
//            });
//        }

//        public void Complete(Guid id)
//        {
//            _repository.SetStatus(id, Domain.Enums.BackgroundJobstatus.Finished);
//        }
//    }
//}
