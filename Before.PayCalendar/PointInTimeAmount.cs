using System;

namespace Before.PayCalendar
{
    /// <summary>
    /// Represents a supplemental mortgage payment for a particular point-in-time
    /// or a theorhetical mortgage balance for a particular point-in-time
    /// </summary>
    public class PointInTimeAmount
    {
        public DateTime ScheduledFor { get; set; }
        public Double   Amount       { get; set; }
    }
}
