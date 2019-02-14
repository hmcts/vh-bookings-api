using System;
using Bookings.Domain.Ddd;

namespace Bookings.Domain
{
    public class ChecklistAnswer : Entity<long>
    {
        protected ChecklistAnswer()
        {
            CreatedAt = DateTime.Now;
        }

        public ChecklistAnswer(ChecklistQuestion question) : this()
        {
            Question = question;
        }

        /// <summary>
        /// The answer to the question, could be a free form string, boolean data or other format depending on the question
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Free form text notes, either describing the answer or data additional to the answer
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Log of when the entry was stored
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The question answered
        /// </summary>
        public virtual ChecklistQuestion Question { get; protected set; }
    }
}