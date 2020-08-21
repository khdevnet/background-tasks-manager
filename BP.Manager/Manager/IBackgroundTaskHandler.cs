
using System.Threading.Tasks;

namespace BP.Manager.Manager
{
    public interface IBackgroundTaskHandler<TBackgroundTask> where TBackgroundTask: struct, IBackgroundTask
    {
        Task Start(BackgroundProcess backgroundProcess, TBackgroundTask backgroundTask);
    }
}
