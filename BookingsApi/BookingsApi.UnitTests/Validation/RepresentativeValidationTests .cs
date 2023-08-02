using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation
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
            request.HearingRoleName = "Representative";
            request.Representee = string.Empty;

            var result = await _representativeValidator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == RepresentativeValidation.NoRepresentee)
                .Should().BeTrue();
        }
     
        [Test]
        public async Task Should_return_missing_organisation_error()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Representative";
            request.OrganisationName = string.Empty;

            var result = await _representativeValidator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == RepresentativeValidation.NoOrganisation)
                .Should().BeTrue();
        }

        private ParticipantRequest BuildRequest()
        {
            return Builder<ParticipantRequest>.CreateNew()
                 .With(x => x.CaseRoleName = "Applicant")
                 .With(x => x.HearingRoleName = "Representative")
                 .Build();
        }
    }
}