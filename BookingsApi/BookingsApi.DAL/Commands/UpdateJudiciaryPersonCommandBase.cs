using BookingsApi.DAL.Commands.Core;

namespace BookingsApi.DAL.Commands
{

    public abstract class UpdateJudiciaryPersonCommandBase : ICommand
    {
        public string ExternalRefId { get; set; }
        public string PersonalCode { get; set; }
        public string Title { get; set; }
        public string KnownAs { get; set; }
        public string Fullname { get; set; }
        public string Surname { get; set; }
        public string PostNominals { get; set; }
        public string Email { get; set; }
        public bool Leaver { get; set; }

        public bool HasLeft => Leaver;

        public string LeftOn { get; set; }
    }
}