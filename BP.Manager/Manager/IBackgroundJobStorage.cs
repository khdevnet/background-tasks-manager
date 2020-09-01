using System;
using System.Collections.Generic;

namespace BP.Manager.Manager
{
    public interface IBackgroundJobStorage
    {
        void Add(BackgroundJob task);
        bool TryRemove(Guid taskId, out BackgroundJob removedTask);
        IReadOnlyCollection<BackgroundJob> Get();
    }
}
