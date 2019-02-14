using System;
using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class BookNewHearingRequestValidation : AbstractValidator<BookNewHearingRequest>
    {
        public static readonly string NoHearingVenueErrorMessage = "Hearing venue cannot not be blank";
        public static readonly string ScheduleDateTimeInPastErrorMessage = "ScheduledDateTime cannot be in the past";
        public static readonly string NoScheduleDurationErrorMessage = "Schedule duration must be greater than 0";
        public static readonly string NoCaseTypeNameErrorMessage = "Please provide a case type name";
        public static readonly string NoHearingTypeErrorMessage = "Please provide a hearing type name";
        public static readonly string NoParticipantsErrorMessage = "Please provide at least one participant";
        public static readonly string NoCasesErrorMessage = "Please provide at least one case";
        
        public BookNewHearingRequestValidation()
        {
            RuleFor(x => x.HearingVenueName)
                .NotEmpty().WithMessage(NoHearingVenueErrorMessage);
           
            RuleFor(x => x.ScheduledDateTime.Date)
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage(ScheduleDateTimeInPastErrorMessage);
            
            RuleFor(x => x.ScheduledDuration)
                .GreaterThan(0).WithMessage(NoScheduleDurationErrorMessage);
            
            RuleFor(x => x.CaseTypeName)
                .NotEmpty().WithMessage(NoCaseTypeNameErrorMessage);
            
            RuleFor(x => x.HearingTypeName)
                .NotEmpty().WithMessage(NoHearingTypeErrorMessage);
            
            RuleFor(x => x.Participants).NotEmpty()
                .NotEmpty().WithMessage(NoParticipantsErrorMessage);
            
            RuleFor(x => x.Cases).NotEmpty()
                .NotEmpty().WithMessage(NoCasesErrorMessage);

            RuleForEach(x => x.Participants)
                .SetValidator(new ParticipantRequestValidation());
            
            RuleForEach(x => x.Cases)
                .SetValidator(new CaseRequestValidation());
        }
    }
}