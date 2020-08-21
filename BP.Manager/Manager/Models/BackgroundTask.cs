using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BP.Manager.Manager
{
    public class BackgroundTask : IDisposable 
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly IServiceScope serviceScope;
        private readonly IServiceProvider serviceProvider;

        public Guid Id { get; }
        public CancellationToken Token { get; }

        public BackgroundTask(Guid id, IServiceProvider serviceProvider)
        {
            Id = id;
            serviceScope = serviceProvider.CreateScope();
            this.serviceProvider = serviceScope.ServiceProvider;
            Token = cts.Token;
        }

        public async Task Start(IBackgroundTaskData data)
        {
            Type generic = typeof(IBackgroundTaskHandler<>);
            Type[] typeArgs = { data.GetType() };
            Type constructed = generic.MakeGenericType(typeArgs);
            dynamic service = serviceProvider.GetService(constructed);
            await service.StartAsync(this, data);
        }

        public void Cancel()
        {
            serviceScope?.Dispose();
            cts.Cancel();
        }

        public void Dispose()
        {
            cts?.Dispose();
            serviceScope?.Dispose();
        }
    }
}
