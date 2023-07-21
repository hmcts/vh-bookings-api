using BookingsApi.DAL.Commands.Core;

namespace BookingsApi.DAL.Commands.V1
{
    public abstract class JudiciaryPersonCommandBase : ICommand
    {
        protected JudiciaryPersonCommandBase(string externalRefId, string personalCode, string title,
            string knownAs, string surname, string fullname, string postNominals, string email, bool hasLeft,
            bool leaver, string leftOn)
        {
            ExternalRefId = externalRefId;
            PersonalCode = personalCode;
            Title = title;
            KnownAs = knownAs;
            Surname = surname;
            Fullname = fullname;
            PostNominals = postNominals;
            Email = email;
            HasLeft = hasLeft;
            Leaver = leaver;
            LeftOn = leftOn;
        }

        public string ExternalRefId { get; }
        public string PersonalCode { get; }
        public string Title { get; }
        public string KnownAs { get; }
        public string Surname { get; }
        public string Fullname { get; }
        public string PostNominals { get; }
        public string Email { get; }
        public bool HasLeft { get; set; }
        public bool Leaver { get; }
        public string LeftOn { get; }
    }
}