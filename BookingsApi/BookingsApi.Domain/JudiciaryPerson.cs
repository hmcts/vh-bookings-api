using System;
using BookingsApi.Domain.Ddd;
using BookingsApi.Domain.Dtos;

namespace BookingsApi.Domain
{
    public class JudiciaryPerson : TrackableAggregateRoot<Guid>
    {
        public JudiciaryPerson(string externalRefId, string personalCode, string title, string knownAs, string surname,
            string fullname, string postNominals, string email, string workPhone, bool hasLeft, bool leaver, string leftOn,
            bool deleted = false, string deletedOn = null)
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
            HasLeft = hasLeft;
            Leaver = leaver;
            LeftOn = leftOn;
            Deleted = deleted;
            DeletedOn = deletedOn;
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
        public bool HasLeft { get; set; }
        public bool Leaver { get; set; }
        public string LeftOn { get; set; }
        public bool IsGeneric { get; set; }
        public bool Deleted { get; private set; }
        public string DeletedOn { get; private set; }

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
            Deleted = command.Deleted;
            DeletedOn = command.DeletedOn;
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