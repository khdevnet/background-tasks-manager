using BP.Manager.Domain.Enums;
using System;

namespace BP.Manager.Domain.Entity
{
 

    public class BackgroundTaskEntity
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string TaskType { get; set; }
        public BackgroundTaskStatus Status { get; set; }
        public string Data { get; set; }
    }
}
