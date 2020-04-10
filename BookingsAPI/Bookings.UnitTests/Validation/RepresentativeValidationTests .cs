using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.UnitTests.Validation
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
        public async Task Should_return_missing_representative_reference_error()
        {
            var request = BuildRequest();
            request.HearingRoleName = "Representative";
            request.Reference = string.Empty;

            var result = await _representativeValidator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == RepresentativeValidation.NoReference)
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
                 .With(x => x.CaseRoleName = "Claimant")
                 .With(x => x.HearingRoleName = "Representative")
                 .Build();
        }
    }
}