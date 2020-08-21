
using System.Threading.Tasks;

namespace BP.Manager.Manager
{
    public interface IBackgroundTaskHandler<TBackgroundTask> where TBackgroundTask: struct, IBackgroundTaskData
    {
        Task Start(BackgroundTask backgroundProcess, TBackgroundTask backgroundTask);
    }
}
