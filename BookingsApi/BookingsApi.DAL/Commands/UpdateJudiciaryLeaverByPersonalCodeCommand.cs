namespace BookingsApi.DAL.Commands;

public class UpdateJudiciaryLeaverByPersonalCodeCommand(string personalCode, bool leaver)
    : JudiciaryLeaverCommandBase(personalCode, leaver);

public class UpdateJudiciaryLeaverByPersonalCodeHandler(BookingsDbContext context)
    : ICommandHandler<UpdateJudiciaryLeaverByPersonalCodeCommand>
{
    public async Task Handle(UpdateJudiciaryLeaverByPersonalCodeCommand command)
    {
        var person = await context.JudiciaryPersons.SingleOrDefaultAsync(x => x.PersonalCode == command.PersonalCode);

        if (person == null)
        {
            throw new JudiciaryLeaverNotFoundException(command.PersonalCode);
        }

        person.Update(command.HasLeft);

        await context.SaveChangesAsync();
    }
}