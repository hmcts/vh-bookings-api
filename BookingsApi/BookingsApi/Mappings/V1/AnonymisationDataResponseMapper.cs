﻿using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings.V1
{
    public static class AnonymisationDataResponseMapper
    {
        public static AnonymisationDataResponse Map(
            AnonymisationDataDto anonymisationDataDto)
        {
            return new AnonymisationDataResponse
            {
                Usernames = anonymisationDataDto.Usernames,
                HearingIds = anonymisationDataDto.HearingIds
            };
        }
    }
}