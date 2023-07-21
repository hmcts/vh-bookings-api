using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetJobHistoryByJobNameQuery : IQuery
    {
        public string JobName { get; }

        public GetJobHistoryByJobNameQuery(string jobName) => JobName = jobName;
        
    }

    public class GetJobHistoryByJobNameQueryHandler : IQueryHandler<GetJobHistoryByJobNameQuery, List<JobHistory>>
    {
        private readonly BookingsDbContext _context;

        public GetJobHistoryByJobNameQueryHandler(BookingsDbContext context) => _context = context;

        public async Task<List<JobHistory>> Handle(GetJobHistoryByJobNameQuery query) => await _context.JobHistory.Where(e => e.JobName == query.JobName).ToListAsync();
        
    }
}