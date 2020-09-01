using BP.Manager.Domain.Enums;
using System;

namespace BP.Manager.Domain.Entity
{
    public class BackgroundJobState
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public string TaskType { get; set; }
        public string TaskDataType { get; set; }
        public BackgroundJobstatus Status { get; set; }
        public string Data { get; set; }
    }
}
