using System;
using System.Collections.Generic;
using System.Linq;

namespace After.PayCalendar
{
    /// <summary>
    /// Defines a simple abstraction for validating supplemental loan payments
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// Returns true if given payment is allowed
        /// (Note: other parameters may optionally be used in the validation process)
        /// </summary>
        Boolean CanApply (Mortgage mortgage, Double balance, DateTime target, Payment payment);
    }

    /// <summary>
    /// Provides methods for computing loan amortization schedules
    /// </summary>
    public static class PayCalendar
    {
        /// <summary>
        /// Calculates balance schedule for a given Mortgage with supplemental payments
        /// (Note: provided rules are used to validate supplemental payments)
        /// </summary>
        public static IEnumerable<Schedule> Compute (IEnumerable<IRule>   rules
                                                    ,Mortgage             mortgage
                                                    ,IEnumerable<Payment> payments)
        {
            var hasPayments  = payments.Count() > 0;
            var currentMonth = new DateTime(mortgage.StartDate.Year, mortgage.StartDate.Month, 1);
            var finalMonth   = currentMonth.AddMonths(mortgage.TermInMonths);
            var balanceLeft  = mortgage.Principal;
   
            while (balanceLeft > 0 && currentMonth < finalMonth)
            {
                var monthlyInterest  = balanceLeft * mortgage.MonthlyRate;
                var monthlyPrincipal = mortgage.MonthlyAmount - monthlyInterest;

                var supplement = 0.0;
                if (hasPayments)
                {
                   supplement = payments.Where (p => rules.All(r => r.CanApply(mortgage
                                                                              ,balanceLeft
                                                                              ,currentMonth
                                                                              ,p)))
                                        .Select(p => p.Amount)
                                        .Sum   ();
                }

                var adjustedBalance = balanceLeft - (monthlyPrincipal + supplement);
                balanceLeft = Math.Max(0,adjustedBalance);

                yield return new Schedule(currentMonth,balanceLeft);
                currentMonth = currentMonth.AddMonths(1);
            }
        }
    }
}
