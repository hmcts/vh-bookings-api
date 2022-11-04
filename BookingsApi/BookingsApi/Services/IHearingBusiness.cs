using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;

namespace BookingsApi.Services
{ 
    public interface IHearingBusiness
    {
        Task<List<VideoHearing>> GetUnallocatedHearings();
    }
}
