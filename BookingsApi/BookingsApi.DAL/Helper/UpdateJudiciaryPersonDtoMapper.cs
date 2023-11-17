using BookingsApi.DAL.Commands;
using BookingsApi.Domain.Dtos;

namespace BookingsApi.DAL.Helper
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
                WorkPhone = command.WorkPhone,
                Leaver = command.Leaver,
                LeftOn = command.LeftOn,
                HasLeft = command.HasLeft
            };
        }
    }
}