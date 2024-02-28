using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class CancelHearingsInGroupRequestHearingRefDataValidation : RefDataInputValidatorValidator<CancelHearingsInGroupRequest>
    {
        public CancelHearingsInGroupRequestHearingRefDataValidation(IReadOnlyCollection<VideoHearing> requestHearings)
        {
            RuleForEach(x=> requestHearings).Custom(ValidateHearing);
        }
        
        private static void ValidateHearing(VideoHearing requestHearing, 
            ValidationContext<CancelHearingsInGroupRequest> context)
        {
            var currentStatus = requestHearing.Status;
            const BookingStatus newStatus = BookingStatus.Cancelled;
            
            var bookingStatusTransition = new BookingStatusTransition();
            var statusChangedEvent = new StatusChangedEvent(currentStatus, newStatus);
        
            if (!bookingStatusTransition.IsValid(statusChangedEvent))
            {
                context.AddFailure($"Cannot change the booking status from {currentStatus} to {newStatus} for hearing {requestHearing.Id}");
            }
        }
    }
}
