using System;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class UpdateJudiciaryPersonByExternalRefIdCommand : ICommand
    {
        public UpdateJudiciaryPersonByExternalRefIdCommand(Guid externalRefId, string personalCode, string title, 
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

        public Guid ExternalRefId { get; set; }
        public string PersonalCode { get; set; }
        public string Title { get; set; }
        public string KnownAs { get; set; }
        public string Surname { get; set; }
        public string Fullname { get; set; }
        public string PostNominals { get; set; }
        public string Email { get; set; }
    }

    public class UpdateJudiciaryPersonByExternalRefIdHandler : ICommandHandler<UpdateJudiciaryPersonByExternalRefIdCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateJudiciaryPersonByExternalRefIdHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(UpdateJudiciaryPersonByExternalRefIdCommand command)
        {
            var person = await _context.JudiciaryPersons.SingleOrDefaultAsync(x => x.ExternalRefId == command.ExternalRefId);

            if (person == null)
            {
                throw new JudiciaryPersonNotFoundException(command.ExternalRefId);
            }

            person.Update(command.PersonalCode, command.Title, command.KnownAs, command.Surname, command.Fullname, command.PostNominals, command.Email);

            await _context.SaveChangesAsync();
        }
    }
}