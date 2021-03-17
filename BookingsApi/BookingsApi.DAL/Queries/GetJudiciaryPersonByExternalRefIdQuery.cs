using System;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetJudiciaryPersonByExternalRefIdQuery : IQuery
    {
        public Guid ExternalRefId { get; set; }

        public GetJudiciaryPersonByExternalRefIdQuery(Guid externalRefId)
        {
            ExternalRefId = externalRefId;
        }
    }
    
    public class GetJudiciaryPersonByExternalRefIdQueryHandler : IQueryHandler<GetJudiciaryPersonByExternalRefIdQuery, JudiciaryPerson>
    {
        private readonly BookingsDbContext _context;

        public GetJudiciaryPersonByExternalRefIdQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<JudiciaryPerson> Handle(GetJudiciaryPersonByExternalRefIdQuery query)
        {
            return await _context.JudiciaryPersons.SingleOrDefaultAsync(x => x.ExternalRefId == query.ExternalRefId);   
            
        }
    }
}