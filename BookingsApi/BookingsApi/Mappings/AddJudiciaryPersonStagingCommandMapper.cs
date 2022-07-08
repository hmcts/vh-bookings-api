using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;

namespace BookingsApi.Mappings
{
    public static class AddJudiciaryPersonStagingCommandMapper
    {
        public static AddJudiciaryPersonStagingCommand Map(
            JudiciaryPersonStagingRequest judiciaryPersonStagingRequest)
        {
            return new AddJudiciaryPersonStagingCommand
            {
                ExternalRefId = judiciaryPersonStagingRequest.Id,
                PersonalCode = judiciaryPersonStagingRequest.PersonalCode,
                Title = judiciaryPersonStagingRequest.Title,
                KnownAs = judiciaryPersonStagingRequest.KnownAs,
                Surname = judiciaryPersonStagingRequest.Surname,
                Fullname = judiciaryPersonStagingRequest.Fullname,
                PostNominals = judiciaryPersonStagingRequest.PostNominals,
                Email = judiciaryPersonStagingRequest.Email,
                Leaver = judiciaryPersonStagingRequest.Leaver,
                LeftOn = judiciaryPersonStagingRequest.LeftOn
            };
        }
    }
}