using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetHearingsByUsernameForDeletionQuery : IQuery
    {
        public GetHearingsByUsernameForDeletionQuery(string username)
        {
            Username = username;
        }

        public string Username { get; }
    }

    public class
        GetHearingsByUsernameForDeletionQueryHandler : IQueryHandler<GetHearingsByUsernameForDeletionQuery,
            List<VideoHearing>>
    {
        private readonly BookingsDbContext _context;

        public GetHearingsByUsernameForDeletionQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<VideoHearing>> Handle(GetHearingsByUsernameForDeletionQuery query)
        {
            var username = query.Username.ToLower().Trim();

            var allHearings = await _context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole).ThenInclude(x => x.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.Person)
                .Include(x => x.HearingCases).ThenInclude(x => x.Case)
                .Include(x => x.CaseType)
                .Include(x => x.HearingVenue)
                .Where(x => x.Participants.Any(p => p.Person.Username.ToLower().Trim() == username))
                .AsNoTracking().ToListAsync();

            var filteredHearings = FilterHearingsWithoutUserAsJudge(allHearings, query.Username);

            // foreach (var videoHearing in filteredHearings)
            // {
            //     RemoveOtherParticipants(videoHearing);
            // }

            return filteredHearings.ToList();
        }

        private static void RemoveOtherParticipants(VideoHearing videoHearing)
        {
            var participants = videoHearing.GetParticipants();
            var participantsToRemove = participants.Where(p =>
                !(p.HearingRole.UserRole.IsIndividual || p.HearingRole.UserRole.IsRepresentative)).ToList();
            foreach (var participant in participantsToRemove)
            {
                videoHearing.RemoveParticipant(participant);
            }
        }

        private IEnumerable<VideoHearing> FilterHearingsWithoutUserAsJudge(List<VideoHearing> allHearings,
            string username)
        {
            foreach (var hearing in allHearings)
            {
                var p = hearing.GetParticipants().First(x =>
                    x.Person.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
                if (!p.HearingRole.UserRole.IsJudge)
                {
                    RemoveOtherParticipants(hearing);
                    yield return hearing;
                }
            }
        }
    }
}