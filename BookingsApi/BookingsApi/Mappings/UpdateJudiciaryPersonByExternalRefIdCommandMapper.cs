﻿using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands;

namespace BookingsApi.Mappings
{
    public static class UpdateJudiciaryPersonByExternalRefIdCommandMapper
    {
        public static UpdateJudiciaryPersonByExternalRefIdCommand Map(JudiciaryPersonRequest request)
        {
            return new UpdateJudiciaryPersonByExternalRefIdCommand
            {
                ExternalRefId = request.Id,
                PersonalCode = request.PersonalCode,
                Title = request.Title,
                KnownAs = request.KnownAs,
                Fullname = request.Fullname,
                Surname = request.Surname,
                PostNominals = request.PostNominals,
                Email = request.Email,
                Leaver = request.Leaver,
                LeftOn = request.LeftOn,
            };
        }
    }
}