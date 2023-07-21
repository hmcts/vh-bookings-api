using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.V1;
using BookingsApi.Domain.Dtos;

namespace BookingsApi.DAL.Mappings
{
    public static class UpdateJudiciaryPersonDtoMapper
    {
        public static UpdateJudiciaryPersonDto Map(UpdateJudiciaryPersonByPersonalCodeCommand command)
        {
            return new UpdateJudiciaryPersonDto
            {
                ExternalRefId = command.ExternalRefId,
                PersonalCode = command.PersonalCode,
                Title = command.Title,
                KnownAs = command.KnownAs,
                Fullname = command.Fullname,
                Surname = command.Surname,
                PostNominals = command.PostNominals,
                Email = command.Email,
                Leaver = command.Leaver,
                LeftOn = command.LeftOn,
                HasLeft = command.HasLeft
            };
        }
    }
}