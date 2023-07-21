using System;
using System.Linq;
using BookingsApi.Contract.V1.Requests;
using Castle.Core.Internal;
using FluentValidation;

namespace BookingsApi.Validations
{
    public class BookNewHearingRequestValidation : AbstractValidator<BookNewHearingRequest>
    {
        public const string HearingVenueErrorMessage = "Hearing venue cannot not be blank";
        public const string ScheduleDateTimeInPastErrorMessage = "ScheduledDateTime cannot be in the past";
        public const string ScheduleDurationErrorMessage = "Schedule duration must be greater than 0";
        public const string CaseTypeNameErrorMessage = "Please provide a case type name";
        public const string HearingTypeNameErrorMessage = "Please provide a hearing type name";
        public const string ParticipantsErrorMessage = "Please provide at least one participant";
        public const string CasesErrorMessage = "Please provide at least one case";
        public const string CaseDuplicationErrorMessage = "Please make sure there are no duplicated cases";
        public const string HearingTypeCodeErrorMessage = "Please provide a hearing type code";

        public BookNewHearingRequestValidation(bool referenceDataFeatureEnabled = false)
        {
            RuleFor(x => x.HearingVenueName)
                .NotEmpty().WithMessage(HearingVenueErrorMessage);

            RuleFor(x => x.ScheduledDateTime.Date)
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage(ScheduleDateTimeInPastErrorMessage);

            RuleFor(x => x.ScheduledDuration)
                .GreaterThan(0).WithMessage(ScheduleDurationErrorMessage);

            RuleFor(x => x.CaseTypeName)
                .NotEmpty().WithMessage(CaseTypeNameErrorMessage);

            if (referenceDataFeatureEnabled)
            {
                RuleFor(x => x.HearingTypeCode)
                    .NotEmpty().WithMessage(HearingTypeCodeErrorMessage);
            }
            else
            {
                RuleFor(x => x.HearingTypeName)
                    .NotEmpty().WithMessage(HearingTypeNameErrorMessage);
            }

            RuleFor(x => x.Participants).NotEmpty()
                .NotEmpty().WithMessage(ParticipantsErrorMessage);

            RuleFor(x => x.Cases).NotEmpty()
                .NotEmpty().WithMessage(CasesErrorMessage);
            
            RuleFor(x => x.Cases)
                .Must(cases => !cases.GroupBy(i => new {i.Number, i.Name}).Any(i => i.Count() > 1))
                .WithMessage(CaseDuplicationErrorMessage);

            RuleForEach(x => x.Participants)
                .SetValidator(new ParticipantRequestValidation());

            RuleForEach(x => x.Cases)
                .SetValidator(new CaseRequestValidation());

            RuleForEach(x => x.LinkedParticipants)
                .SetValidator(new LinkedParticipantRequestValidation()).When(x => !x.LinkedParticipants.IsNullOrEmpty());

        }
    }
}