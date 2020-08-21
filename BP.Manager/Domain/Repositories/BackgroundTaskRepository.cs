using BP.Manager.Domain.Database;
using BP.Manager.Domain.Entity;
using BP.Manager.Domain.Enums;
using BP.Manager.Manager;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BP.Manager.Domain.Repositories
{
    public class BackgroundTaskRepository
    {
        private readonly TaskStatesContext db;

        public BackgroundTaskRepository(TaskStatesContext db)
        {
            this.db = db;
        }

        public async Task Create(BackgroundTaskEntity backgroundTask)
        {
            await db.BackgroundTasks.AddAsync(backgroundTask);
            db.SaveChanges();
        }

        public async Task SetStatus(Guid id, BackgroundTaskStatus status)
        {
            var task = await db.BackgroundTasks.FindAsync(id);
            task.Status = status;
            db.SaveChanges();
        }

        public async Task Remove(Guid id)
        {
            var task = await db.BackgroundTasks.FindAsync(id);
            db.BackgroundTasks.Remove(task);
            db.SaveChanges();
        }

        public async Task<IEnumerable<BackgroundTaskEntity>> Get()
        {
            return await db.BackgroundTasks.ToListAsync();
        }
    }
}
