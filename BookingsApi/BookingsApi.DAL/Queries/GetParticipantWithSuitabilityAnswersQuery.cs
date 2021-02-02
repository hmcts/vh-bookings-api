using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain.Participants;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetParticipantWithSuitabilityAnswersQuery : IQuery
    {
        private int _limit;

        public GetParticipantWithSuitabilityAnswersQuery()
        {
            Limit = 100;
        }

        public string Cursor { get; set; }

        public int Limit
        {
            get => _limit;
            set
            {
                if (value <= 0) throw new ArgumentException("Limit must be one or more", nameof(value));
                _limit = value;
            }
        }
    }

    public class GetParticipantWithSuitabilityAnswersQueryHandler : IQueryHandler<GetParticipantWithSuitabilityAnswersQuery,
            CursorPagedResult<Participant, string>>
    {
        private readonly BookingsDbContext _context;

        public GetParticipantWithSuitabilityAnswersQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<CursorPagedResult<Participant, string>> Handle(GetParticipantWithSuitabilityAnswersQuery query)
        {
            IQueryable<Participant> participants = _context.Participants
                .Include("Person")
                .Include("HearingRole")
                .Include("Hearing.HearingCases.Case")
                .Include("Questionnaire")
                .Include("Questionnaire.SuitabilityAnswers");

            participants = participants.Where(x => x.Questionnaire != null && x.Questionnaire.SuitabilityAnswers.Any()).OrderByDescending(x => x.Questionnaire.UpdatedDate).ThenBy(x => x.Id.ToString());
            if (!string.IsNullOrEmpty(query.Cursor))
            {
                TryParseCursor(query.Cursor, out var updatedDateTime, out var id);
                participants = participants.Where(x => x.Questionnaire.UpdatedDate <= updatedDateTime
                                               &&  x.Id != Guid.Parse(id));
            }

          
            // Add one to the limit to know whether or not we have a next page
            var result = await participants.Take(query.Limit + 1).ToListAsync();
            string nextCursor = null;
            if (result.Count > query.Limit)
            {
                // The next cursor should be built based on the last item in the list
                result = result.Take(query.Limit).ToList();
                var lastResult = result.Last();
                nextCursor = $"{lastResult.Questionnaire.UpdatedDate.Ticks}_{lastResult.Id}";
            }

            return new CursorPagedResult<Participant, string>(result, nextCursor);
        }

        private void TryParseCursor(string cursor, out DateTime updatedDateTime, out string id)
        {
            try
            {
                var parts = cursor.Split('_');
                updatedDateTime = new DateTime(long.Parse(parts[0]));
                id = parts[1];
            }
            catch (Exception e)
            {
                throw new FormatException($"Unexpected cursor format [{cursor}]", e);
            }
        }
    }
}
