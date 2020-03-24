using System;
using System.Linq;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using Bookings.UnitTests.Utilities;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Hearing
{
    public class AddParticipantTests : TestBase
    {
        [Test]
        public void Should_add_new_participant_to_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var claimantCaseRole = new CaseRole(1, "Claimant");
            var claimantSolicitorHearingRole = new HearingRole(2, "Solicitor");

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;
            hearing.AddSolicitor(newPerson, claimantSolicitorHearingRole, claimantCaseRole, "Display Name",
                string.Empty, string.Empty);

            var afterAddCount = hearing.GetParticipants().Count;

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }

        [Test]
        public void Should_not_add_existing_participant_to_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var representative = (Representative) hearing.GetParticipants().First(x => x.GetType() == typeof(Representative));
            var beforeAddCount = hearing.GetParticipants().Count;

            Action action = () => hearing.AddSolicitor(representative.Person, representative.HearingRole,
                representative.CaseRole, representative.DisplayName, representative.SolicitorsReference,
                representative.Representee);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == "Participant already exists in the hearing").Should().BeTrue();
            
            var afterAddCount = hearing.GetParticipants().Count;
            afterAddCount.Should().Be(beforeAddCount);
        }

        [Test]
        public void Should_add_judge_to_hearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            var judgeCaseRole = new CaseRole(5, "Judge");
            var judgeHearingRole = new HearingRole(13, "Judge");

            var newPerson = new PersonBuilder(true).Build();
            var beforeAddCount = hearing.GetParticipants().Count;

            hearing.AddJudge(newPerson, judgeHearingRole, judgeCaseRole, "Judge Display Name");
            var afterAddCount = hearing.GetParticipants().Count;

            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }

        [Test]
        public void Should_raise_exception_if_adding_judge_twice()
        {
            var hearingBuilder = new VideoHearingBuilder();
            var hearing = hearingBuilder.Build();
            var existingJudge = hearingBuilder.Judge;
            
            var judgeCaseRole = new CaseRole(5, "Judge");
            var judgeHearingRole = new HearingRole(13, "Judge");
            var newPerson = new PersonBuilder(existingJudge.Username).Build();

            When(() => hearing.AddJudge(newPerson, judgeHearingRole, judgeCaseRole, "Judge Dredd"))
                .Should().Throw<DomainRuleException>().WithMessage("Judge with given username already exists in the hearing");

        }
    }
}