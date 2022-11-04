using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common;
using BookingsApi.Common.Const;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;

namespace BookingsApi.Services
{ 
    public class HearingBusiness : IHearingBusiness
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ILogger _logger;
        private readonly List<string> ScottishHearingVenuesList = new List<string> { 
            HearingScottishVenueNames.Aberdeen,
            HearingScottishVenueNames.Ayr,
            HearingScottishVenueNames.Dundee,
            HearingScottishVenueNames.Edinburgh,
            HearingScottishVenueNames.Glasgow,
            HearingScottishVenueNames.HamiltonBrandonGate,
            HearingScottishVenueNames.Inverness,
            HearingScottishVenueNames.StirlingWallaceHouse,
            HearingScottishVenueNames.EdinburghEmploymentAppealTribunal,
            HearingScottishVenueNames.InvernessJusticeCentre,
            HearingScottishVenueNames.EdinburghSocialSecurityAndChildSupportTribunal,
            HearingScottishVenueNames.EdinburghUpperTribunal,
        };
        
        public HearingBusiness(IQueryHandler queryHandler, ILogger logger)
        {
            _queryHandler = queryHandler;
            _logger = logger;
        }
        public async Task<List<VideoHearing>> GetUnallocatedHearings()
        {
            _logger.TrackEvent("GetUnallocatedHearings started");
            var startDate = DateTime.Today; 
            var endDate = DateTime.Today.AddDays(30);    // 30 days is 1 month.
            
            
            var query = new GetHearingsNotAllocatedQuery(startDate, endDate);
            var results = await _queryHandler.Handle<GetHearingsNotAllocatedQuery, List<VideoHearing>>(query);
            
            results = results.Where(x=> !ScottishHearingVenuesList.Any(venueName => venueName == x.HearingVenueName)).ToList();

            _logger.TrackEvent("GetUnallocatedHearings completed");
            return results;
        }
    }
}
