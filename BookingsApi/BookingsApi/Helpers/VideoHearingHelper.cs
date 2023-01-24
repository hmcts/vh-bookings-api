﻿using BookingsApi.Contract.Requests;
using BookingsApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.DAL.Helper;
using Newtonsoft.Json;
using FluentValidation.Results;

namespace BookingsApi.Helpers
{
    public static class VideoHearingHelper
    {
        public static string AllocatedVho(Hearing videoHearing, bool retrieveContactEmail = true)
        {
            var allocatedVho = "Not Allocated";
            var isScottishVenue =
                HearingScottishVenueNames.ScottishHearingVenuesList.Any(venueName =>
                    venueName == videoHearing.HearingVenueName);
            if (videoHearing.AllocatedTo == null) {
                if (isScottishVenue || videoHearing.CaseTypeId == 3) // not required if scottish venue or generic type
                {
                    allocatedVho = "Not Required";
                }
            } else {
                if (retrieveContactEmail)
                {
                    allocatedVho = videoHearing.AllocatedTo.ContactEmail;
                }
                else
                {
                    allocatedVho = videoHearing.AllocatedTo.FirstName + " " + videoHearing.AllocatedTo.Lastname;
                }
            }
            return allocatedVho;
        }
    }
}
