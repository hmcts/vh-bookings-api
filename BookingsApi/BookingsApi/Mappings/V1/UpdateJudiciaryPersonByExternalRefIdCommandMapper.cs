﻿using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Mappings.V1
{
    public static class UpdateJudiciaryPersonByExternalRefIdCommandMapper
    {
        public static UpdateJudiciaryPersonByPersonalCodeCommand Map(JudiciaryPersonRequest request)
        {
            return new UpdateJudiciaryPersonByPersonalCodeCommand
            {
                ExternalRefId = request.Id,
                PersonalCode = request.PersonalCode,
                Title = request.Title,
                KnownAs = request.KnownAs,
                Fullname = request.Fullname,
                Surname = request.Surname,
                PostNominals = request.PostNominals,
                Email = request.Email,
                WorkPhone = request.WorkPhone,
                Leaver = request.Leaver,
                LeftOn = request.LeftOn,
                Deleted = request.Deleted,
                DeletedOn = request.DeletedOn
            };
        }
    }
}