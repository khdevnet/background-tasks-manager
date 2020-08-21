using System.Threading.Tasks;

namespace BP.Manager.Manager
{


    public abstract class BackgroundTaskHandlerBase<TBackgroundTask> : IBackgroundTaskHandler<TBackgroundTask> where TBackgroundTask : IBackgroundTaskData
    {
        protected abstract Task StartAsync(BackgroundTask bt, TBackgroundTask data);

        public async Task StartAsync(BackgroundTask bt, IBackgroundTaskData data)
        {
            await StartAsync(bt, (TBackgroundTask)data);
        }
    }
}
