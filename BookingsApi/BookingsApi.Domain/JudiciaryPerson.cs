using System;
using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain
{
    public class JudiciaryPerson : AggregateRoot<Guid>
    {
        public JudiciaryPerson(Guid externalRefId, string personalCode, string title, string knownAs, string surname,
            string fullname, string postNominals, string email)
        {
            Id = Guid.NewGuid();
        }

        public Guid ExternalRefId { get; set; }
        public string PersonalCode { get; set; }
        public string Title { get; set; }
        public string KnownAs { get; set; }
        public string Surname { get; set; }
        public string Fullname { get; set; }
        public string PostNominals { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
    }
}