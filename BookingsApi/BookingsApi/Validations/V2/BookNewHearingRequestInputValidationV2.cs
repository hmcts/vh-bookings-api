using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.V1;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class BookNewHearingRequestInputValidationV2 : AbstractValidator<BookNewHearingRequestV2>
    {
        public const string ScheduleDateTimeInPastErrorMessage = "ScheduledDateTime cannot be in the past";
        public const string ScheduleDurationErrorMessage = "Schedule duration must be greater than 0";
        public const string CaseTypeServiceIdErrorMessage = "Please provide a case type service ID";
        public const string ParticipantsErrorMessage = "Please provide at least one participant";
        public const string CasesErrorMessage = "Please provide at least one case";
        public const string CaseDuplicationErrorMessage = "Please make sure there are no duplicated cases";
        public const string HearingTypeCodeErrorMessage = "Please provide a hearing type code";
        public const string HearingVenueCodeErrorMessage = "Please provide a hearing venue code";

        public BookNewHearingRequestInputValidationV2()
        {
            RuleFor(x => x.HearingVenueCode)
                .NotEmpty().WithMessage(HearingVenueCodeErrorMessage);
            RuleFor(x => x.HearingTypeCode)
                .NotEmpty().WithMessage(HearingTypeCodeErrorMessage);
            RuleFor(x => x.ServiceId)
                .NotEmpty().WithMessage(CaseTypeServiceIdErrorMessage);
            
            RuleFor(x => x.ScheduledDateTime).Custom((dateTime, context) =>
            {
                if (dateTime < DateTime.UtcNow)
                {
                    context.AddFailure(ScheduleDateTimeInPastErrorMessage);
                }
            });

            RuleFor(x => x.ScheduledDuration)
                .GreaterThan(0).WithMessage(ScheduleDurationErrorMessage);

            RuleFor(x => x.Participants).NotEmpty()
                .NotEmpty().WithMessage(ParticipantsErrorMessage);

            RuleFor(x => x.Cases).NotEmpty()
                .NotEmpty().WithMessage(CasesErrorMessage);

            RuleFor(x => x.Cases)
                .Must(cases => !cases.GroupBy(i => new {i.Number, i.Name}).Any(i => i.Count() > 1))
                .WithMessage(CaseDuplicationErrorMessage);

            RuleForEach(x => x.Participants)
                .SetValidator(new ParticipantRequestValidationV2());

            RuleForEach(x => x.Cases)
                .SetValidator(new CaseRequestValidationV2());

            RuleForEach(x => x.LinkedParticipants)
                .SetValidator(new LinkedParticipantRequestValidationV2())
                .When(x => !x.LinkedParticipants.Any());

            RuleForEach(x => x.Endpoints)
                .SetValidator(new EndpointRequestValidationV2())
                .When(x => x.Endpoints.Any());
            
            RuleForEach(request =>  request.JudiciaryParticipants).SetValidator(new JudiciaryParticipantRequestValidation());
            RuleFor(x => x.CreatedBy).NotEmpty();
        }
    }
}