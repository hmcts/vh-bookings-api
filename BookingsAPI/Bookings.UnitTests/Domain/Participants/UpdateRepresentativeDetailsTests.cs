using Bookings.Domain.Participants;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Participants
{
    public class UpdateRepresentativeDetailsTests
    {
        [Test]
        public void should_update_participant_with_user_role_representative_details()
        {
            var participant = new ParticipantBuilder().RepresentativeParticipantDefendant;
            var representative = (Representative)participant;
            string representee = "Representee Edit";
            string solicitorsReference = "Test Reference Edit";
           
            representative.UpdateRepresentativeDetails(solicitorsReference,representee);
            representative.SolicitorsReference.Should().Be(solicitorsReference);
            representative.Representee.Should().Be(representee);
                      
        }

        [Test]
        public void should_throw_exception_when_validation_fails()
        {
            var participant = new ParticipantBuilder().RepresentativeParticipantDefendant;
            var representativeParticipant = (Representative)participant;
            string representee = "";
            string solicitorsReference = "";
            var beforeUpdatedDate = representativeParticipant.UpdatedDate;

            Action action = () => representativeParticipant.UpdateRepresentativeDetails(solicitorsReference, representee);
            action.Should().Throw<DomainRuleException>()
                .And.ValidationFailures.Should()
                .Contain(x => x.Name == "SolicitorsReference").And
                .Contain(x => x.Name == "Representee");

            representativeParticipant.UpdatedDate.Should().Be(beforeUpdatedDate);
        }
    }
}
