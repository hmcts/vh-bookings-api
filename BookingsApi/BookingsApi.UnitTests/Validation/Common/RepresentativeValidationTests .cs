using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain.Constants;
using BookingsApi.Validations.Common;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Validation.Common
{
    public class RepresentativeValidationTests
    {
         private RepresentativeValidation _representativeValidator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
           _representativeValidator = new RepresentativeValidation();
        }


        [Test]
        public async Task Should_return_missing_representee_error()
        {
            var request = BuildRequest();
            request.HearingRoleCode = HearingRoleCodes.Representative;
            request.Representee = string.Empty;

            var result = await _representativeValidator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Exists(x => x.ErrorMessage == RepresentativeValidation.NoRepresentee)
                .Should().BeTrue();
        }
     
        [Test]
        public async Task Should_not_return_missing_organisation_error()
        {
            var request = BuildRequest();
            request.HearingRoleCode = HearingRoleCodes.Representative;
            request.OrganisationName = string.Empty;

            var result = await _representativeValidator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        private static ParticipantRequestV2 BuildRequest()
        {
            return Builder<ParticipantRequestV2>.CreateNew()
                 .With(x => x.HearingRoleCode = HearingRoleCodes.Representative)
                 .Build();
        }
    }
}