using System;

namespace Before.PayCalendar
{
    /// <summary>
    /// Defines a simple abstraction for validating supplemental loan payments
    /// </summary>
    public abstract class RuleBase
    {
        public Mortgage          Mortgage { get; set; }
        public Double            Balance  { get; set; }
        public DateTime          Target   { get; set; }
        public PointInTimeAmount Payment  { get; set; }

        public abstract Boolean CanApply ();
    }
}
