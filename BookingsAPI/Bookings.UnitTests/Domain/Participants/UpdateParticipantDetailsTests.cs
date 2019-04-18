using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Domain;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Hearing
{
    public class UpdateParticipantDetailsTests
    {


        [Test]
        public void should_update_participant_with_user_role_individual_details()
        {
            var individualParticipant = new ParticipantBuilder().IndividualPrticipantClaimant;
            var displayName = "Edit Display Name";
            var telephoneNumber = "111122223";
            var title = "Edit Title";
            var houseNumber = "Edit 1";
            var street = "Edit Street";
            var city = "Edit City";
            var county = "Edit County";
            var postcode = "ED1 5NR";
            var organisationName = "Edit Org1";
            var beforeUpdatedDate = individualParticipant.UpdatedDate;

            individualParticipant.UpdateParticipantDetails(title, displayName, telephoneNumber, street, houseNumber, city, county, postcode, organisationName);
            individualParticipant.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            individualParticipant.Person.Title.Should().Be(title);
            individualParticipant.DisplayName.Should().Be(displayName);
            individualParticipant.Person.TelephoneNumber.Should().Be(telephoneNumber);
            individualParticipant.Person.Address.HouseNumber.Should().Be(houseNumber);
            individualParticipant.Person.Address.Street.Should().Be(street);
            individualParticipant.Person.Address.Postcode.Should().Be(postcode);
        }

        [Test]
        public void should_update_participant_with_user_role_representative_details()
        {
            var representativeParticipant = new ParticipantBuilder().RepresentativeParticipantDefendant;
            var displayName = "Edit Display Name";
            var telephoneNumber = "111122223";
            var title = "Edit Title";
            var houseNumber = "Edit 1";
            var street = "Edit Street";
            var city = "Edit City";
            var county = "Edit County";
            var postcode = "ED1 5NR";
            var organisationName = "Edit Org1";
            var beforeUpdatedDate = representativeParticipant.UpdatedDate;

            representativeParticipant.UpdateParticipantDetails(title, displayName, telephoneNumber, street, houseNumber, city, county, postcode, organisationName);
            representativeParticipant.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            representativeParticipant.Person.Title.Should().Be(title);
            representativeParticipant.DisplayName.Should().Be(displayName);
            representativeParticipant.Person.TelephoneNumber.Should().Be(telephoneNumber);
            representativeParticipant.Person.Address.Should().Be(null);
        }

        [Test]
        public void should_throw_exception_when_validation_fails()
        {
            var individualParticipant = new ParticipantBuilder().IndividualPrticipantClaimant;
            var displayName = "";
            var telephoneNumber = "111122223";
            var title = "Edit Title";
            var houseNumber = "Edit 1";
            var street = "Edit Street";
            var city = "Edit City";
            var county = "Edit County";
            var postcode = "ED1 5NR";
            var organisationName = "Edit Org1";
            var beforeUpdatedDate = individualParticipant.UpdatedDate;

            Action action = () => individualParticipant.UpdateParticipantDetails(title, displayName, telephoneNumber, street, houseNumber, city, county, postcode, organisationName); ;
            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "DisplayName");

            individualParticipant.UpdatedDate.Should().Be(beforeUpdatedDate);
        }
    }
}