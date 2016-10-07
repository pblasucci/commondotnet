using System;

namespace After.PayCalendar
{
    /// <summary>
    /// Represents a supplemental mortgage payment for a particular point-in-time
    /// </summary>
    public sealed class Payment
    {
        private readonly DateTime postDate_;
        private readonly Double   amount_;
                
        public Payment(DateTime scheduledFor, Double amount)
        {
            postDate_ = scheduledFor;
            amount_   = amount;
        }

        public DateTime PostDate { get { return postDate_; } }
        public Double   Amount   { get { return amount_;   } }

    }

    /// <summary>
    /// Represents a theorhetical mortgage balance for a particular point-in-time
    /// </summary>
    public sealed class Schedule
    {
        private readonly DateTime dueDate_;
        private readonly Double   balance_;
        
        internal Schedule (DateTime dueDate, Double balance)
        {
            dueDate_ = dueDate;
            balance_ = balance;
        }

        public DateTime DueDate { get { return dueDate_; } }
        public Double   Balance { get { return balance_; } }
    }
}
