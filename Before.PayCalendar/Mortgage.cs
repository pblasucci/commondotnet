using System;

namespace Before.PayCalendar
{
    /// <summary>
    /// Encapsulates the basic details of a mortgage contract
    /// </summary>
    public class Mortgage
    {
        public Double   Principal        { get; set; }
        public Double   AnnualPercentage { get; set; }
        public Int32    TermInMonths     { get; set; }
        public DateTime StartDate        { get; set; }
        
        public Double MonthlyRate { get { return AnnualPercentage / 12; } }
        public Double MonthlyAmount
        {
            get
            {
                var denominator = 1 - Math.Pow(1 + MonthlyRate, -TermInMonths);
                return (Principal * MonthlyRate) / denominator;
            }
        }
    }
}
