using BookingsApi.Domain;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IBookingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing);
    }
}
