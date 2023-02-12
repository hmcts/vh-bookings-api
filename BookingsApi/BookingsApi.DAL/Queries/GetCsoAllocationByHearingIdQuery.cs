using System;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetCsoAllocationByHearingIdQuery : IQuery
    {
        public Guid HearingId { get;}

        public GetCsoAllocationByHearingIdQuery(Guid hearingId)
        {
            HearingId = hearingId;
        }
    }

    public class GetCsoAllocationByHearingIdQueryHandler : IQueryHandler<GetCsoAllocationByHearingIdQuery, JusticeUser>
    {
        private readonly BookingsDbContext _context;

        public GetCsoAllocationByHearingIdQueryHandler(BookingsDbContext context) => _context = context;
        
        public async Task<JusticeUser> Handle(GetCsoAllocationByHearingIdQuery query)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.Allocations)
                    .ThenInclude(a => a.JusticeUser)
                    .ThenInclude(ju => ju.UserRole)
                .FirstOrDefaultAsync(x => x.Id == query.HearingId);
            return hearing?.AllocatedTo;
        }
    }
}