using System;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetHearingShellByIdQuery : IQuery
    {
        public Guid HearingId { get; set; }

        public GetHearingShellByIdQuery(Guid hearingId)
        {
            HearingId = hearingId;
        }
    }

    public class GetHearingShellByIdQueryHandler : IQueryHandler<GetHearingShellByIdQuery, VideoHearing>
    {
        private readonly BookingsDbContext _context;

        public GetHearingShellByIdQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<VideoHearing> Handle(GetHearingShellByIdQuery query)
        {
            return await _context.VideoHearings
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.HearingId);
        }
    }
}