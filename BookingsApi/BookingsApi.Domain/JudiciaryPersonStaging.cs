using System;
using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain
{
    public class JudiciaryPersonStaging : TrackableAggregateRoot<Guid>
    {
        public JudiciaryPersonStaging(string externalRefId, string personalCode, string title, string knownAs, string surname,
            string fullname, string postNominals, string email, string workPhone, string leaver, string leftOn, 
            bool deleted = false, string deletedOn = null) : this()
        {
            ExternalRefId = externalRefId;
            PersonalCode = personalCode;
            Title = title;
            KnownAs = knownAs;
            Surname = surname;
            Fullname = fullname;
            PostNominals = postNominals;
            Email = email;
            WorkPhone = workPhone;
            Leaver = leaver;
            LeftOn = leftOn;
            Deleted = deleted;
            DeletedOn = deletedOn;
        }

        protected JudiciaryPersonStaging()
        {
            Id = Guid.NewGuid();
        }

        public string ExternalRefId { get; set; }
        public string PersonalCode { get; set; }
        public string Title { get; set; }
        public string KnownAs { get; set; }
        public string Fullname { get; set; }
        public string Surname { get; set; }
        public string PostNominals { get; set; }
        public string Email { get; set; }
        public string WorkPhone { get; set; }
        public string Leaver { get; set; }
        public string LeftOn { get; set; }
        public bool Deleted { get; private set; }
        public string DeletedOn { get; private set; }
    }
}