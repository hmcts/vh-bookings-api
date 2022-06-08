using System;
using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain
{
    public class JudiciaryPerson : AggregateRoot<Guid>
    {
        private readonly DateTime _currentUTC = DateTime.UtcNow;
        public JudiciaryPerson(Guid externalRefId, string personalCode, string title, string knownAs, string surname,
            string fullname, string postNominals, string email, bool hasLeft)
        {
            Id = Guid.NewGuid();
            ExternalRefId = externalRefId;
            PersonalCode = personalCode;
            Title = title;
            KnownAs = knownAs;
            Surname = surname;
            Fullname = fullname;
            PostNominals = postNominals;
            Email = email;
            CreatedDate = _currentUTC;
            UpdatedDate = _currentUTC;
            HasLeft = hasLeft;
        }

        public Guid ExternalRefId { get; set; }
        public string PersonalCode { get; set; }
        public string Title { get; set; }
        public string KnownAs { get; set; }
        public string Surname { get; set; }
        public string Fullname { get; set; }
        public string PostNominals { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; }
        public DateTime UpdatedDate { get; private set; }
        public bool HasLeft { get; set; }

        public void Update(string personalCode, string title, string knownAs, string surname, string fullname, 
            string postNominals, string email, bool hasLeft)
        {
            PersonalCode = personalCode;
            Title = title;
            KnownAs = knownAs;
            Surname = surname;
            Fullname = fullname;
            PostNominals = postNominals;
            Email = email;
            UpdatedDate = DateTime.UtcNow;
            HasLeft = hasLeft;
        }

        public void Update(bool hasLeft)
        {
            UpdatedDate = DateTime.UtcNow;
            HasLeft = hasLeft;

            if (hasLeft)
            {
                Fullname = null;
                KnownAs = null;
                Surname = null;
                Email = null;
                Title = null;
                PostNominals = null;
            }
            
        }
    }
}