﻿using BookingsApi.DAL.Helper;

namespace BookingsApi.DAL.Commands
{
    public class NewEndpoint
    {
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public string ContactEmail { get; set; } 
    }
    
    public class AddEndPointToHearingCommand : ICommand
    {
        public AddEndPointToHearingCommand(Guid hearingId, NewEndpoint endpoint, string createdBy = "")
        {
            HearingId = hearingId;
            Endpoint = endpoint;
            CreatedBy = createdBy;
        }

        public Guid HearingId { get; }
        public NewEndpoint Endpoint { get; }
        public string CreatedBy { get; set; }
    }

    public class AddEndPointToHearingCommandHandler : ICommandHandler<AddEndPointToHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public AddEndPointToHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddEndPointToHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.Participants).ThenInclude(x => x.Person)
                .Include(h => h.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var dto = command.Endpoint;
            var defenceAdvocate = DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(dto.ContactEmail, hearing.GetParticipants());
            var endpoint = new Endpoint(dto.DisplayName, dto.Sip, dto.Pin, defenceAdvocate);
            hearing.AddEndpoint(endpoint, createdBy: command.CreatedBy);
            await _context.SaveChangesAsync();
        }
    }
}
