using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Domain.Participants;

namespace Bookings.DAL.Helper
{
    public static class DefenceAdvocateHelper
    {
        public static Participant CheckAndReturnDefenceAdvocate(string defenceAdvocateUsername,
            IEnumerable<Participant> participants)
        {
            Participant defenceAdvocate = null;

            if (!string.IsNullOrWhiteSpace(defenceAdvocateUsername))
            {
                defenceAdvocate = participants.First(x =>
                    x.Person.Username.Equals(defenceAdvocateUsername, StringComparison.CurrentCultureIgnoreCase));
            }

            return defenceAdvocate;
        }
    }
}