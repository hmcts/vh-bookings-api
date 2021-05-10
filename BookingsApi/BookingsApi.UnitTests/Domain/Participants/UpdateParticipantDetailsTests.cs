using System;
using BookingsApi.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Domain.Participants
{
    public class UpdateParticipantDetailsTests
    {


        [Test]
        public void Should_update_participant_with_user_role_individual_details()
        {
            var individualParticipant = new ParticipantBuilder().IndividualParticipantApplicant;
            var displayName = "Edit Display Name";
            var telephoneNumber = "111122223";
            var title = "Edit Title";
            var organisationName = "Edit Org1";
            var beforeUpdatedDate = individualParticipant.UpdatedDate;

            individualParticipant.UpdateParticipantDetails(title, displayName, telephoneNumber, organisationName);
            individualParticipant.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            individualParticipant.Person.Title.Should().Be(title);
            individualParticipant.DisplayName.Should().Be(displayName);
            individualParticipant.Person.TelephoneNumber.Should().Be(telephoneNumber);
        }

        [Test]
        public void Should_update_participant_with_user_role_representative_details()
        {
            var representativeParticipant = new ParticipantBuilder().RepresentativeParticipantRespondent;
            var displayName = "Edit Display Name";
            var telephoneNumber = "111122223";
            var title = "Edit Title";
            var organisationName = "Edit Org1";
            var beforeUpdatedDate = representativeParticipant.UpdatedDate;

            representativeParticipant.UpdateParticipantDetails(title, displayName, telephoneNumber, organisationName);
            representativeParticipant.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
            representativeParticipant.Person.Title.Should().Be(title);
            representativeParticipant.DisplayName.Should().Be(displayName);
            representativeParticipant.Person.TelephoneNumber.Should().Be(telephoneNumber);
        }

        [Test]
        public void Should_throw_exception_when_validation_fails()
        {
            var individualParticipant = new ParticipantBuilder().IndividualParticipantApplicant;
            var displayName = "";
            var telephoneNumber = "111122223";
            var title = "Edit Title";
            var organisationName = "Edit Org1";
            var beforeUpdatedDate = individualParticipant.UpdatedDate;

            Action action = () => individualParticipant.UpdateParticipantDetails(title, displayName, telephoneNumber, organisationName);
            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "DisplayName");

            individualParticipant.UpdatedDate.Should().Be(beforeUpdatedDate);
        }

        [Test]
        public void Should_use_existing_title_if_new_title_is_NULL()
        {
            //Arrange
            var individualParticipant = new ParticipantBuilder().IndividualParticipantApplicant;
            var displayName = "Edit Display Name";
            var telephoneNumber = "111122223";
            string title = null;
            var organisationName = "Edit Org1";

            //Act
            individualParticipant.UpdateParticipantDetails(title, displayName, telephoneNumber, organisationName);

            individualParticipant.Person.Title.Should().NotBeNull();
        }

        [Test]
        public void Should_use_new_title_if_new_title_is_not_NULL()
        {
            //Arrange
            var individualParticipant = new ParticipantBuilder().IndividualParticipantApplicant;
            var displayName = "Edit Display Name";
            var telephoneNumber = "111122223";
            string title = "New Title";
            var organisationName = "Edit Org1";

            //Act
            individualParticipant.UpdateParticipantDetails(title, displayName, telephoneNumber, organisationName);

            individualParticipant.Person.Title.Should().Be(title);
        }
		
		[Test]
        public void Should_use_existing_title_if_new_telephone_is_NULL()
        {
            //Arrange
            var individualParticipant = new ParticipantBuilder().IndividualParticipantApplicant;
            var displayName = "Edit Display Name";
            string telephoneNumber = null;
            string title = "new Title";
            var organisationName = "Edit Org1";

            //Act
            individualParticipant.UpdateParticipantDetails(title, displayName, telephoneNumber, organisationName);

            individualParticipant.Person.TelephoneNumber.Should().NotBeNull();
        }

        [Test]
        public void Should_use_new_telephone_if_new_telephonenumber_is_not_NULL()
        {
            //Arrange
            var individualParticipant = new ParticipantBuilder().IndividualParticipantApplicant;
            var displayName = "Edit Display Name";
            var telephoneNumber = "1111220000223";
            string title = "New Title";
            var organisationName = "Edit Org1";

            //Act
            individualParticipant.UpdateParticipantDetails(title, displayName, telephoneNumber, organisationName);

            individualParticipant.Person.TelephoneNumber.Should().Be(telephoneNumber);
        }

    }
}