using System;
using System.Linq;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Consts;
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
        public const string HearingTypeErrorMessage = "Please provide a hearing type name";
        public const string ParticipantsErrorMessage = "Please provide at least one participant";
        public const string CasesErrorMessage = "Please provide at least one case";
        public const string CaseDuplicationErrorMessage = "Please make sure there are no duplicated cases";
        public const string JudgeAbsenceErrorMessage = "A judge must be included in a hearing";

        public BookNewHearingRequestValidation()
        {
            RuleFor(x => x.HearingVenueName)
                .NotEmpty().WithMessage(HearingVenueErrorMessage);

            RuleFor(x => x.ScheduledDateTime.Date)
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage(ScheduleDateTimeInPastErrorMessage);

            RuleFor(x => x.ScheduledDuration)
                .GreaterThan(0).WithMessage(ScheduleDurationErrorMessage);

            RuleFor(x => x.CaseTypeName)
                .NotEmpty().WithMessage(CaseTypeNameErrorMessage);

            RuleFor(x => x.HearingTypeName)
                .NotEmpty().WithMessage(HearingTypeErrorMessage);

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

            RuleFor(x => x.Participants)
                .Must(participant => participant.Any(x => x.HearingRoleName == HearingRoleName.Judge))
                .WithMessage(JudgeAbsenceErrorMessage);
        }
    }
}