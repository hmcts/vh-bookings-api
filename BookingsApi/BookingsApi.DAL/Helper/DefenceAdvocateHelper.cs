using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Helper
{
    public static class DefenceAdvocateHelper
    {
        public static Participant CheckAndReturnDefenceAdvocate(string defenceAdvocateContactEmail,
            IEnumerable<Participant> participants)
        {
            Participant defenceAdvocate = null;

            if (!string.IsNullOrWhiteSpace(defenceAdvocateContactEmail))
            {
                defenceAdvocate = participants.First(x =>
                    x.Person.ContactEmail.Equals(defenceAdvocateContactEmail, StringComparison.CurrentCultureIgnoreCase));
            }

            return defenceAdvocate;
        }
    }
}