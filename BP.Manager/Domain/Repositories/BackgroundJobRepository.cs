using BP.Manager.Domain.Database;
using BP.Manager.Domain.Entity;
using BP.Manager.Domain.Enums;
using BP.Manager.Manager;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.Manager.Domain.Repositories
{
    public class BackgroundJobRepository
    {
        private readonly BackgroundJobsContext db;

        public BackgroundJobRepository(BackgroundJobsContext db)
        {
            this.db = db;
        }

        public void Create(Entity.BackgroundJobState backgroundTask)
        {
            db.BackgroundJobs.Add(backgroundTask);
            db.SaveChanges();
        }

        public void SetStatus(Guid id, BackgroundJobstatus status)
        {
            var task = db.BackgroundJobs.Find(id);
            task.Status = status;
            db.SaveChanges();
        }

        public void  Remove(Guid id)
        {
            var task = db.BackgroundJobs.Find(id);
            db.BackgroundJobs.Remove(task);
            db.SaveChanges();
        }

        public IEnumerable<Entity.BackgroundJobState> Get()
        {
            return db.BackgroundJobs.ToList();
        }
    }
}
