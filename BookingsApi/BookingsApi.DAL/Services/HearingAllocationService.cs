using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Configuration;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookingsApi.DAL.Services
{
    public interface IHearingAllocationService
    {
        Task<JusticeUser> AllocateAutomatically(Guid hearingId);
        Task<JusticeUser> AllocateManually(Guid hearingId, Guid justiceUserCsoId);
        Task DeallocateFromUnavailableHearings(Guid justiceUserId);
        void CheckAndDeallocateHearing(Hearing hearing);
        Task<List<VideoHearing>> AllocateHearingsToCso(List<Guid> postRequestHearings, Guid postRequestCsoId);
    }

    public class HearingAllocationService : IHearingAllocationService
    {
        private readonly BookingsDbContext _context;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly AllocateHearingConfiguration _configuration;
        private readonly ILogger<HearingAllocationService> _logger;

        public HearingAllocationService(
            BookingsDbContext context, 
            IRandomNumberGenerator randomNumberGenerator,
            IOptions<AllocateHearingConfiguration> configuration,
            ILogger<HearingAllocationService> logger)
        {
            _context = context;
            _randomNumberGenerator = randomNumberGenerator;
            _configuration = configuration.Value;
            _logger = logger;
        }

        /// <summary>
        /// Allocate a list of hearings to a passed CSO.
        /// </summary>
        /// <param name="postRequestHearings">List of hearings to be allocated</param>
        /// <param name="postRequestCsoId">CSO JusticeUser Id</param>
        /// <exception cref="DomainRuleException"></exception>
        public async Task<List<VideoHearing>> AllocateHearingsToCso(List<Guid> postRequestHearings, Guid postRequestCsoId)
        {
            try
            {
                foreach (Guid guid in postRequestHearings)
                {
                    await AllocateManually(guid, postRequestCsoId);
                }
                
                return await GetListOfHearings(postRequestHearings);
            }
            catch (DomainRuleException e)
            {
                throw new DomainRuleException("NoCsosAvailable",
                    e.Message);
            }
        }

        /// <summary>
        /// Allocate automatically a hearing to a random CSO
        /// </summary>
        /// <param name="hearingId">hearing Id to be allocated</param>
        /// <param name="justiceUserCsoId">CSO JusticeUser Id</param>
        /// <returns>CSO allocated User</returns>
        /// <exception cref="DomainRuleException"></exception>
        public async Task<JusticeUser> AllocateAutomatically(Guid hearingId)
        {
            VideoHearing hearing = await GetHearing(hearingId);
            
            var allocations = _context.Allocations.Where(a => a.HearingId == hearing.Id).ToList();
            
            // only in automatic allocation we check if hearing has been allocated
            if (allocations.Any())
            {
                throw new DomainRuleException("HearingAlreadyAllocated",
                    $"Hearing {hearing.Id} has already been allocated");
            }
            
            JusticeUser cso = SelectCso(hearing);
            if (cso == null)
            {
                throw new DomainRuleException("NoCsosAvailable",
                    $"Unable to allocate to hearing {hearingId}, no CSOs available");
            }
            
            await AllocateHearingToCso(cso, hearing);

            return cso;
        }
        
        /// <summary>
        /// Allocate manually to a passed CSO
        /// </summary>
        /// <param name="hearingId">hearing Id to be allocated</param>
        /// <param name="justiceUserCsoId">CSO JusticeUser Id</param>
        /// <returns>CSO allocated User</returns>
        /// <exception cref="DomainRuleException"></exception>
        public async Task<JusticeUser> AllocateManually(Guid hearingId, Guid justiceUserCsoId)
        {
            VideoHearing hearing = await GetHearing(hearingId);
            
            var allocations = _context.Allocations.Where(a => a.HearingId == hearing.Id).ToList();
            
            JusticeUser cso = null;
            if (allocations.Any())
            {
                // we need to unallocate the hearing and allocate to the new user
                hearing.Deallocate();
                _logger.LogInformation("Deallocated hearing {hearingId}", hearing.Id);
            }
            cso = GetCso((Guid) justiceUserCsoId);
            if (cso == null)
            {
                throw new DomainRuleException("CsoNotFound",
                    $"Unable to allocate to hearing {hearingId}, with CSO {justiceUserCsoId}");
            }

            await AllocateHearingToCso(cso, hearing);

            return cso;
        }
        
        private async Task<List<VideoHearing>> GetListOfHearings(List<Guid> postRequestHearings)
        {
            List<VideoHearing> list = new List<VideoHearing>();

            foreach (Guid id in postRequestHearings)
            {
                VideoHearing hearing = await GetHearing(id);
                list. Add(hearing);
            }

            return list;
        }

        private async Task<VideoHearing> GetHearing(Guid hearingId)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.CaseType)
                .Include(h => h.HearingType)
                .Include(h => h.HearingCases).ThenInclude(hc => hc.Case)
                .Include(h => h.Allocations).ThenInclude(a => a.JusticeUser)
                .SingleOrDefaultAsync(x => x.Id == hearingId);
            if (hearing == null)
            {
                throw new DomainRuleException("HearingNotFound",
                    $"Hearing {hearingId} not found");
            }

            return hearing;
        }

        private async Task AllocateHearingToCso(JusticeUser cso, VideoHearing hearing)
        {
            _context.Allocations.Add(new Allocation
            {
                Hearing = hearing,
                JusticeUser = cso
            });
            await _context.SaveChangesAsync();
            _logger.LogInformation("Allocated hearing {hearingId} to cso {cso}", hearing.Id, cso.Username);
        }

        /// <summary>
        /// Deallocates the user from any hearings they are no longer available for
        /// </summary>
        /// <param name="justiceUserId"></param>
        /// <exception cref="DomainRuleException"></exception>
        public async Task DeallocateFromUnavailableHearings(Guid justiceUserId)
        {
            var user = await _context.JusticeUsers
                .Include(x => x.Allocations).ThenInclude(x => x.Hearing)
                .Include(x => x.VhoWorkHours)
                .Include(x => x.VhoNonAvailability)
                .SingleOrDefaultAsync(x => x.Id == justiceUserId);
            if (user == null)
            {
                throw new DomainRuleException("JusticeUserNotFound",
                    $"Justice user {justiceUserId} not found");
            }

            var allocations = user.Allocations
                .Where(x => x.Hearing.ScheduledDateTime > DateTime.UtcNow)
                .ToList();
            
            foreach (var hearing in allocations.Select(allocation => allocation.Hearing).ToList())
            {
                if (!user.IsAvailable(hearing.ScheduledDateTime, hearing.ScheduledEndTime, _configuration))
                {
                    hearing.Deallocate();
                    _logger.LogInformation("Deallocated hearing {hearingId}", hearing.Id);
                }
            }
        }
        
        /// <summary>
        /// Deallocates the user from the hearing if they are not available
        /// </summary>
        /// <param name="hearing"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public void CheckAndDeallocateHearing(Hearing hearing)
        {
            var allocatedUser = hearing.AllocatedTo;
            if (allocatedUser == null)
            {
                return;
            }
            
            if (!allocatedUser.IsAvailable(hearing.ScheduledDateTime, hearing.ScheduledEndTime, _configuration))
            {
                hearing.Deallocate();
                _logger.LogInformation("Deallocated hearing {hearingId}", hearing.Id);
            }
        }

        private JusticeUser SelectCso(Hearing hearing)
        {
            var availableCsos = GetAvailableCsos(hearing);

            if (!availableCsos.Any())
            {
                return null;
            }

            if (availableCsos.Count == 1)
            {
                return availableCsos.SingleOrDefault();
            }

            var csos = availableCsos
                .Select(c => new
                {
                    Cso = c,
                    AllocationCount = c.Allocations?.Count ?? 0
                })
                .ToList();
            
            var lowestAllocationCount = csos.Min(c => c.AllocationCount);
            var csosWithFewestAllocations = csos.Where(c => c.AllocationCount == lowestAllocationCount).ToList();

            if (csosWithFewestAllocations.Count == 1)
            {
                return csosWithFewestAllocations.Single().Cso;
            }

            return SelectRandomly(csosWithFewestAllocations.Select(c => c.Cso).ToList());
        }

        private List<JusticeUser> GetAvailableCsos(Hearing hearing)
        {
            var csos = _context.JusticeUsers
                .Include(u => u.UserRole)
                .Where(u => u.UserRoleId == (int)UserRoleId.Vho)
                .Include(u => u.VhoWorkHours)
                .Include(u => u.VhoNonAvailability)
                .Include(u => u.Allocations).ThenInclude(a => a.Hearing)
                .ToList();

            return csos
                .Where(cso => hearing.CanAllocate(cso, _configuration))
                .ToList();
        }
        
        private JusticeUser GetCso(Guid id)
        {
            var cso = _context.JusticeUsers
                .Include(u => u.UserRole)
                .Where(u => u.Id == id)
                .Include(u => u.VhoWorkHours)
                .Include(u => u.VhoNonAvailability)
                .Include(u => u.Allocations).ThenInclude(a => a.Hearing)
                .FirstOrDefault();

            return cso;
        }

        private JusticeUser SelectRandomly(IList<JusticeUser> csos)
        {
            var csoIndex = _randomNumberGenerator.Generate(1, csos.Count);
            var cso = csos[csoIndex-1];
            return cso;
        }
    }
}
