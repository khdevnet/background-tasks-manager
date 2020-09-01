using BP.Manager.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BP.Manager.Manager
{
    public abstract class BackgroundJobBase<TData> : BackgroundJob where TData : IBackgroundJobData
    {
        protected abstract Task Run(TData data);

        public override BackgroundJob Run(IBackgroundJobData data)
        {
            Task.Run(async () =>
            {
                try
                {
                    Data = data;
                    Status = BackgroundJobstatus.Started;
                    onStarted?.Invoke(this);
                    await Run((TData)data);

                    onCompleted?.Invoke(this);
                    Status = BackgroundJobstatus.Finished;
                    Dispose();
                }
                catch (Exception ex)
                {
                    throw;
                }

            });

            return this;
        }
    }

    public abstract class BackgroundJob : IDisposable
    {
        protected readonly CancellationTokenSource cts = new CancellationTokenSource();
        protected IServiceScope serviceScope;
        protected IServiceProvider serviceProvider;

        protected Action<BackgroundJob> onStarted;
        protected Action<BackgroundJob> onCompleted;

        public CancellationToken Token { get; protected set; }

        public Guid Id { get; set; }
        public BackgroundJobstatus Status { get; set; } = BackgroundJobstatus.Started;
        public IBackgroundJobData Data { get; set; }

        public BackgroundJob Configure(Guid id, IServiceScope serviceScope)
        {
            Id = id;
            this.serviceScope = serviceScope;
            serviceProvider = serviceScope.ServiceProvider;
            Token = cts.Token;
            return this;
        }

        protected TService GetService<TService>()
        {
            return serviceProvider.GetService<TService>();
        }

        public BackgroundJob onStart(Action<BackgroundJob> onStarted)
        {
            this.onStarted += onStarted;
            return this;
        }

        public BackgroundJob onComplete(Action<BackgroundJob> onCompleted)
        {
            this.onCompleted += onCompleted;
            return this;
        }

        public abstract BackgroundJob Run(IBackgroundJobData data);

        public void Stop()
        {
            serviceScope?.Dispose();
            cts.Cancel();
        }

        public void Dispose()
        {
            onStarted = null;
            onCompleted = null;
            cts?.Dispose();
            serviceScope?.Dispose();
        }
    }
}
