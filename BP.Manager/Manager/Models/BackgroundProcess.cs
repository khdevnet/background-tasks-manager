using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BP.Manager.Manager
{
    public class BackgroundProcess : IDisposable 
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly IServiceScope serviceScope;
        private readonly IServiceProvider serviceProvider;

        public Guid Id { get; }
        public CancellationToken Token { get; }

        public BackgroundProcess(Guid id, IServiceProvider serviceProvider)
        {
            Id = id;
            serviceScope = serviceProvider.CreateScope();
            this.serviceProvider = serviceScope.ServiceProvider;
            Token = cts.Token;
        }

        public async Task Start<TBackgroundTask>(TBackgroundTask data) where TBackgroundTask: struct, IBackgroundTask
        {
            await serviceProvider.GetService<IBackgroundTaskHandler<TBackgroundTask>>().Start(this, data);
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
