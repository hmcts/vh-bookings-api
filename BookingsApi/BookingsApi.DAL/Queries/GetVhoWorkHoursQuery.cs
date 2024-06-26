﻿namespace BookingsApi.DAL.Queries
{
    public class GetVhoWorkHoursQuery : IQuery
    {
        public string UserName { get; }
        public GetVhoWorkHoursQuery(string userName) => UserName = userName.ToLowerInvariant();
        
    }

    public class GetVhoWorkHoursQueryHandler : IQueryHandler<GetVhoWorkHoursQuery, List<VhoWorkHours>>
    {
        private readonly BookingsDbContext _context;

        public GetVhoWorkHoursQueryHandler(BookingsDbContext context) => _context = context;

        public async Task<List<VhoWorkHours>> Handle(GetVhoWorkHoursQuery query)
        {
            var justiceUser = await _context.JusticeUsers
                .Include(e => e.VhoWorkHours).ThenInclude(e=>e.DayOfWeek)
                .FirstOrDefaultAsync(e => e.Username == query.UserName);
            
            if (justiceUser == null)
            {
                throw new JusticeUserNotFoundException(query.UserName);
            }
            
            return justiceUser.VhoWorkHours?.ToList() ?? new List<VhoWorkHours>();
        }
         
    }
}
