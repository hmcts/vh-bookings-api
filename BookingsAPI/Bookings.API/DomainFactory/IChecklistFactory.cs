using System.Linq;
using Bookings.Domain;
using Bookings.Domain.Participants;

namespace Bookings.API.DomainFactory
{
    /// <summary>
    /// Responsible for dehydrating checklists for participants using stored questions
    /// </summary>
    public interface IChecklistFactory
    {
        Checklist GetForParticipant(Participant participant);

        /// <summary>
        /// Retrieve all the participants having a checklist submitted
        /// </summary>
        /// <remarks>
        /// Order descending by date of checklist submission
        /// </remarks>
        IQueryable<Participant> GetAllParticipantsWithChecklist();
    }
}