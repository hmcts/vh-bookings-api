using System;
using BookingsApi.Domain.Ddd;
using BookingsApi.Domain.Dtos;

namespace BookingsApi.Domain
{
    public class JudiciaryPerson : AggregateRoot<Guid>
    {
        private readonly DateTime _currentUTC = DateTime.UtcNow;
        public JudiciaryPerson(string externalRefId, string personalCode, string title, string knownAs, string surname,
            string fullname, string postNominals, string email, string workPhone, bool hasLeft, bool leaver, string leftOn,
            bool isGeneric = false)
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
            WorkPhone = workPhone;
            CreatedDate = _currentUTC;
            UpdatedDate = _currentUTC;
            HasLeft = hasLeft;
            Leaver = leaver;
            LeftOn = leftOn;
            IsGeneric = isGeneric;
        }

        public string ExternalRefId { get; set; }
        public string PersonalCode { get; set; }
        public string Title { get; set; }
        public string KnownAs { get; set; }
        public string Surname { get; set; }
        public string Fullname { get; set; }
        public string PostNominals { get; set; }
        public string Email { get; set; }
        public string WorkPhone { get; set; }
        public DateTime CreatedDate { get; }
        public DateTime UpdatedDate { get; private set; }
        public bool HasLeft { get; set; }
        public bool Leaver { get; set; }
        public string LeftOn { get; set; }
        public bool IsGeneric { get; set; }

        public void Update(string personalCode, string title, string knownAs, string surname, string fullname, 
            string postNominals, string email, string workPhone, bool hasLeft, bool leaver, string leftOn)
        {
            PersonalCode = personalCode;
            Title = title;
            KnownAs = knownAs;
            Surname = surname;
            Fullname = fullname;
            PostNominals = postNominals;
            Email = email;
            WorkPhone = workPhone;
            UpdatedDate = DateTime.UtcNow;
            HasLeft = hasLeft;
            Leaver = leaver;
            LeftOn = leftOn;
        }
        
        public void Update(UpdateJudiciaryPersonDto command)
        {
            ExternalRefId = command.ExternalRefId;
            PersonalCode = command.PersonalCode;
            Title = command.Title;
            KnownAs = command.KnownAs;
            Surname = command.Surname;
            Fullname = command.Fullname;
            PostNominals = command.PostNominals;
            Email = command.Email;
            WorkPhone = command.WorkPhone;
            UpdatedDate = DateTime.UtcNow;
            HasLeft = command.HasLeft;
            Leaver = command.Leaver;
            LeftOn = command.LeftOn;
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
                WorkPhone = null;
                PostNominals = null;
            }
            
        }
        
        public bool IsALeaver()
        {
            return Leaver || HasLeft;
        }
    }
}