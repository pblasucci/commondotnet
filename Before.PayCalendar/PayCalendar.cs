using System;
using System.Collections.Generic;
using System.Linq;

namespace Before.PayCalendar
{
    /// <summary>
    /// Provides a means for computing loan amortization schedules;
    /// Inheritors should, at a minimum, override Rules which govern payment validation
    /// </summary>
    public abstract class PayCalendar
    {
        protected abstract List<RuleBase> Rules { get; }

        public Mortgage Mortgage { get; set; }

        public List<PointInTimeAmount> Payments { get; set; }

        public List<PointInTimeAmount> Schedule { get; private set; }

        public virtual void Compute ()
        {
            this.Schedule = new List<PointInTimeAmount>();

            var hasPayments  = Payments.Count() > 0;
            var currentMonth = new DateTime(Mortgage.StartDate.Year, Mortgage.StartDate.Month, 1);
            var finalMonth   = currentMonth.AddMonths(Mortgage.TermInMonths);
            var balanceLeft  = Mortgage.Principal;
   
            while (balanceLeft > 0 && currentMonth < finalMonth)
            {
                var monthlyInterest  = balanceLeft * Mortgage.MonthlyRate;
                var monthlyPrincipal = Mortgage.MonthlyAmount - monthlyInterest;

                var supplement = 0.0;
                if (hasPayments)
                {
                   supplement = Payments.Where (p => Rules.All(r => r.CanApply()))
                                        .Select(p => p.Amount)
                                        .Sum   ();
                }

                var adjustedBalance = balanceLeft - (monthlyPrincipal + supplement);
                balanceLeft = Math.Max(0,adjustedBalance);

                this.Schedule.Add(new PointInTimeAmount
                {
                    ScheduledFor = currentMonth,
                    Amount       = balanceLeft
                });
                currentMonth = currentMonth.AddMonths(1);
            }
        }
    }
}
