using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands.V1
{
    public class UpdateJudiciaryPersonByPersonalCodeCommand : ICommand
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

    public class UpdateJudiciaryPersonByPersonalCodeHandler : ICommandHandler<UpdateJudiciaryPersonByPersonalCodeCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateJudiciaryPersonByPersonalCodeHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateJudiciaryPersonByPersonalCodeCommand command)
        {
            var person =
                await _context.JudiciaryPersons.SingleOrDefaultAsync(x => x.PersonalCode == command.PersonalCode);

            if (person == null)
            {
                throw new JudiciaryPersonNotFoundException(command.PersonalCode);
            }

            person.Update(UpdateJudiciaryPersonDtoMapper.Map(command));

            await _context.SaveChangesAsync();
        }
    }
}