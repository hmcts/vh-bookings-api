
using Bookings.Api.Contract.Requests;
using Bookings.API.Helpers;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Bookings.UnitTests.Validation
{
    public class AddressValidationHelperTests
    {
        private List<string> roles;
        private List<ParticipantRequest> participantRequests;

        [SetUp]
        public void TestInitialize()
        {
            roles = new List<string> { "Claimant", "Solicitor" };

            participantRequests = new List<ParticipantRequest>
            {
                new ParticipantRequest
                {
                    HearingRoleName = "Solicitor",
                    HouseNumber = "123A",
                    Street = "Test Street",
                    Postcode = "SW1V 1AB",
                    City = "Westminister",
                    County = "London"
                }
            };
        }

        [Test]
        public void Should_validate_given_address_and_return_true()
        {
            var result = AddressValidationHelper.ValidateAddress(roles, participantRequests);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void Should_validate_given_address_and_return_false()
        {
            participantRequests.Add(new ParticipantRequest
                                        {
                                            HearingRoleName = "Claimant"
                                        });

            var result = AddressValidationHelper.ValidateAddress(roles, participantRequests);

            result.IsValid.Should().BeFalse();
        }

    }
}
