namespace BookingsApi.DAL.Commands
{
    public abstract class JudiciaryPersonCommandBase : ICommand
    {
        protected JudiciaryPersonCommandBase(string externalRefId, string personalCode, string title,
            string knownAs, string surname, string fullname, string postNominals, string email, string workPhone, bool hasLeft,
            bool leaver, string leftOn, bool deleted, string deletedOn)
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
            HasLeft = hasLeft;
            Leaver = leaver;
            LeftOn = leftOn;
            Deleted = deleted;
            DeletedOn = deletedOn;
        }

        public string ExternalRefId { get; }
        public string PersonalCode { get; }
        public string Title { get; }
        public string KnownAs { get; }
        public string Surname { get; }
        public string Fullname { get; }
        public string PostNominals { get; }
        public string Email { get; }
        public string WorkPhone { get; }
        public bool HasLeft { get; set; }
        public bool Leaver { get; }
        public string LeftOn { get; }
        public bool Deleted { get; set; }
        public string DeletedOn { get; set; }
    }
}