using System;
using BookingsApi.DAL.Commands.Core;

namespace BookingsApi.DAL.Commands
{
    public abstract class JudiciaryPersonCommandBase : ICommand
    {
        protected JudiciaryPersonCommandBase(Guid externalRefId, string personalCode, string title, 
            string knownAs, string surname, string fullname, string postNominals, string email)
        {
            ExternalRefId = externalRefId;
            PersonalCode = personalCode;
            Title = title;
            KnownAs = knownAs;
            Surname = surname;
            Fullname = fullname;
            PostNominals = postNominals;
            Email = email;
        }
        
        public Guid ExternalRefId { get; }
        public string PersonalCode { get; }
        public string Title { get; }
        public string KnownAs { get; }
        public string Surname { get; }
        public string Fullname { get; }
        public string PostNominals { get; }
        public string Email { get; }
    }
}