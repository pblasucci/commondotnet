using System;

namespace After.PayCalendar
{
    /// <summary>
    /// Encapsulates the basic details of a mortgage contract
    /// </summary>
    public sealed class Mortgage
    {
        private readonly Double   principal_;
        private readonly Double   annualPercentage_;
        private readonly Double   monthlyRate_;
        private readonly Double   monthlyAmount_;
        private readonly Int32    termInMonths_;
        private readonly DateTime startDate_;
        
        public Mortgage (Double   principal
                        ,Double   annualPercentage
                        ,Int32    termInMonths
                        ,DateTime startDate)
        {
            principal_        = principal;
            annualPercentage_ = annualPercentage;
            termInMonths_     = termInMonths;
            monthlyRate_      = annualPercentage / 12;
            monthlyAmount_    = (principal * monthlyRate_) 
                                / 
                                (1 - Math.Pow(1 + monthlyRate_, -termInMonths));
            startDate_        = startDate;   
        }

        public Double   Principal        { get { return principal_;        } }
        public Double   AnnualPercentage { get { return annualPercentage_; } }
        public Double   MonthlyRate      { get { return monthlyRate_;      } }
        public Double   MonthlyAmount    { get { return monthlyAmount_;    } }
        public Int32    TermInMonths     { get { return termInMonths_;     } }
        public DateTime StartDate        { get { return startDate_;        } }
    }
}
