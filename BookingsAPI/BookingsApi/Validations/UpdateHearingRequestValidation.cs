using System;
using BookingsApi.Contract.Requests;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class UpdateHearingRequestValidation : AbstractValidator<UpdateHearingRequest>
    {
        public static readonly string NoHearingNameErrorMessage = "Hearing name cannot not be blank";
        public static readonly string ScheduleDateTimeInPastErrorMessage = "ScheduledDateTime cannot be in the past";
        public static readonly string NoScheduleDurationErrorMessage = "Schedule duration must be greater than 0";
        public static readonly string NoUpdatedByErrorMessage = "UpdatedBy is missing";
        
        public UpdateHearingRequestValidation()
        {
            RuleFor(x => x.HearingVenueName)
                .NotEmpty().WithMessage(NoHearingNameErrorMessage);
           
            RuleFor(x => x.ScheduledDateTime.Date)
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage(ScheduleDateTimeInPastErrorMessage);
            
            RuleFor(x => x.ScheduledDuration)
                .GreaterThan(0).WithMessage(NoScheduleDurationErrorMessage);
            
            RuleFor(x => x.UpdatedBy)
                .NotEmpty().WithMessage(NoUpdatedByErrorMessage);
        }
    }
}