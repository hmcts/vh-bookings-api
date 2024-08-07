﻿namespace BookingsApi.DAL.Commands
{
    public class AddJudiciaryPersonStagingCommand : ICommand 
    {
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
        public bool Deleted { get; set; }
        public string DeletedOn { get; set; }
    }

    public class AddJudiciaryPersonStagingCommandHandler : ICommandHandler<AddJudiciaryPersonStagingCommand>
    {
        private readonly BookingsDbContext _context;

        public AddJudiciaryPersonStagingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        public async Task Handle(AddJudiciaryPersonStagingCommand command)
        {
            await _context.JudiciaryPersonsStaging.AddAsync(new JudiciaryPersonStaging(command.ExternalRefId,
                command.PersonalCode, command.Title, command.KnownAs, command.Surname, command.Fullname,
                command.PostNominals, command.Email, command.WorkPhone, command.Leaver, command.LeftOn, 
                command.Deleted, command.DeletedOn));

            await _context.SaveChangesAsync();
        }
    }
}