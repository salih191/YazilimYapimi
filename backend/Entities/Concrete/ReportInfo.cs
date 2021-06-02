using System;
using Core.Entities;

namespace Entities.Concrete
{
    public class ReportInfo:IEntity
    {
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}