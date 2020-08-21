
using System.Threading.Tasks;

namespace BP.Manager.Manager
{
    public interface IBackgroundTaskHandler<TBackgroundTask> where TBackgroundTask: IBackgroundTaskData
    {
        Task StartAsync(BackgroundTask backgroundProcess, IBackgroundTaskData backgroundTask);
    }
}
