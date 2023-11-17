using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Mappings.V1
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
                WorkPhone = judiciaryPersonStagingRequest.WorkPhone,
                Leaver = judiciaryPersonStagingRequest.Leaver,
                LeftOn = judiciaryPersonStagingRequest.LeftOn
            };
        }
    }
}