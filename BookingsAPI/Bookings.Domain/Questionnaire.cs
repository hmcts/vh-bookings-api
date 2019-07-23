using Bookings.Domain.Ddd;
using Bookings.Domain.Participants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookings.Domain
{
    public class Questionnaire : Entity<long>
    {
        public Questionnaire()
        {
            SuitabilityAnswers = new List<SuitabilityAnswer>();
            UpdatedDate = DateTime.UtcNow;
        }

        public Guid ParticipantId { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Participant Participant { get; set; }
        public virtual IList<SuitabilityAnswer> SuitabilityAnswers { get; protected set; }

        public void AddSuitabilityAnswers(IList<SuitabilityAnswer> suitabilityAnswers)
        {
            foreach (var suitabilityAnswer in suitabilityAnswers)
            {
                AddSuitabilityAnswer(suitabilityAnswer.Key, suitabilityAnswer.Data, suitabilityAnswer.ExtendedData);
            }
        }

        public virtual void AddSuitabilityAnswer(string key, string data, string extendedData)
        {
            var existingSuitabilityAnswer = SuitabilityAnswers.FirstOrDefault(answer => answer.Key == key);
            if (existingSuitabilityAnswer == null)
            {
                // Add a new answer to collection
                var newSuitabilityAnswer = new SuitabilityAnswer(key, data, extendedData)
                {
                    Questionnaire = this
                };

                SuitabilityAnswers.Add(newSuitabilityAnswer);
            }
            else
            {
                // Update the existing object in the collection
                existingSuitabilityAnswer.Data = data;
                existingSuitabilityAnswer.ExtendedData = extendedData;
                existingSuitabilityAnswer.UpdatedDate = DateTime.UtcNow;
            }

            UpdatedDate = DateTime.UtcNow;
        }
    }
}
