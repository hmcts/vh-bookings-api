using Bookings.Domain.Participants;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using System;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Participants
{
    public class UpdateRepresentativeDetailsTests
    {
        [Test]
        public void Should_update_participant_with_user_role_representative_details()
        {
            var participant = new ParticipantBuilder().RepresentativeParticipantDefendant;
            var representative = (Representative)participant;
            string representee = "Representee Edit";
            string reference = "Test Reference Edit";
           
            representative.UpdateRepresentativeDetails(reference,representee);
            representative.Reference.Should().Be(reference);
            representative.Representee.Should().Be(representee);
                      
        }

        [Test]
        public void Should_throw_exception_when_validation_fails()
        {
            var participant = new ParticipantBuilder().RepresentativeParticipantDefendant;
            var representativeParticipant = (Representative)participant;
            string representee = "";
            string reference = "";
            var beforeUpdatedDate = representativeParticipant.UpdatedDate;

            Action action = () => representativeParticipant.UpdateRepresentativeDetails(reference, representee);
            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "Reference").And
                .Contain(x => x.Name == "Representee");

            representativeParticipant.UpdatedDate.Should().Be(beforeUpdatedDate);
        }
    }
}
