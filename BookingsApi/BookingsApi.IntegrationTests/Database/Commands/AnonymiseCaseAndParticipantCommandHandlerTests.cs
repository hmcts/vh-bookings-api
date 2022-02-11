using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AnonymiseCaseAndParticipantCommandHandlerTests : DatabaseTestsBase
    {
        private AnonymiseCaseAndParticipantCommandHandler _commandHandler;
        private GetAnonymisationDataQueryHandler _getAnonymisationDataQueryHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AnonymiseCaseAndParticipantCommandHandler(context);
            _getAnonymisationDataQueryHandler = new GetAnonymisationDataQueryHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public async Task Anonymise_Case_Name_And_Participant_Display_Name_For_Hearings_Older_Than_3_Months()
        {
            var seededHearing = await Hooks.SeedPastHearings(DateTime.Today.AddMonths(-3));
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var hearingIdsForAnonymisations =
                (await _getAnonymisationDataQueryHandler.Handle(new GetAnonymisationDataQuery())).HearingIds;

            await _commandHandler.Handle(new AnonymiseCaseAndParticipantCommand
                { HearingIds = hearingIdsForAnonymisations });
            
            var returnedVideoHearingAfterFirstAnonymisationRequest = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var hearingDataAfterFirstAnonymisationRequest = returnedVideoHearingAfterFirstAnonymisationRequest.HearingCases[0];
            
            //assertions for case names
            hearingDataAfterFirstAnonymisationRequest.Case.Name.Should().NotBe(seededHearing.HearingCases[0].Case.Name);
            hearingDataAfterFirstAnonymisationRequest.Case.Name.Should().Contain(AnonymiseCaseAndParticipantCommandHandler.AnonymisedNameSuffix);
            
            foreach (var participant in seededHearing.Participants)
            {
                var updatedParticipant = returnedVideoHearingAfterFirstAnonymisationRequest.Participants.FirstOrDefault(p => p.Id == participant.Id);
                updatedParticipant.DisplayName.Should().NotBe(participant.DisplayName);
                updatedParticipant.DisplayName.Should().Contain(AnonymiseCaseAndParticipantCommandHandler.AnonymisedNameSuffix);
            }
            
            //should ignore the same hearing ids for the anonymisation command
            await _commandHandler.Handle(new AnonymiseCaseAndParticipantCommand
                { HearingIds = hearingIdsForAnonymisations });
            var returnedVideoHearingAfterSecondAnonymisationRequest = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var hearingDataAfterSecondAnonymisationRequest = returnedVideoHearingAfterSecondAnonymisationRequest.HearingCases[0];
            
            //assertion for case name
            hearingDataAfterSecondAnonymisationRequest.Case.Name.Should()
                .Be(hearingDataAfterFirstAnonymisationRequest.Case.Name);
            
            //assertions for participant display name
            foreach (var participant in returnedVideoHearingAfterFirstAnonymisationRequest.Participants)
            {
                var matchingParticipant = returnedVideoHearingAfterSecondAnonymisationRequest.Participants.FirstOrDefault(p => p.Id == participant.Id);
                matchingParticipant.DisplayName.Should().Be(participant.DisplayName);
            }
        }
    }
}