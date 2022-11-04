using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.BaseQueries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetHearingsNotAllocatedQuery : IQuery
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public GetHearingsNotAllocatedQuery(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

    }

    public class GetHearingsNotAllocatedQueryHandler : IQueryHandler<GetHearingsNotAllocatedQuery, List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingsNotAllocatedQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsNotAllocatedQuery query)
        {
           
            var hearings =  _context.VideoHearings.Where(x =>
                (x.Status == Domain.Enumerations.BookingStatus.Created || x.Status == Domain.Enumerations.BookingStatus.Booked)
                && x.Status != Domain.Enumerations.BookingStatus.Cancelled
                && x.ScheduledDateTime >= query.StartDate
                && x.ScheduledDateTime < query.EndDate
                && x.CaseTypeId != 3); // Generic Case Type

                var unAllocatedHearings =   
                hearings.Where(x => 
                    _context.Allocations.FirstOrDefault(a => a.HearingId == x.Id) == null).OrderBy(x=>x.ScheduledDateTime);
            
            return await unAllocatedHearings.ToListAsync();
        }
    }
}