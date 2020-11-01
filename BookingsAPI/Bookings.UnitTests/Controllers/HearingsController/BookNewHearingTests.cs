using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.DAL.Commands;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.RefData;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;

namespace Bookings.UnitTests.Controllers.HearingsController
{
    public class BookNewHearingTests : HearingsControllerTest
    {
        private static List<ParticipantRequest> Participants
        {
            get
            {
                var participants = Builder<ParticipantRequest>.CreateListOfSize(5).All()
                .With(x => x.Title = "Mrs")
                .With(x => x.FirstName = $"Automation_{Faker.Name.First()}")
                .With(x => x.LastName = $"Automation_{Faker.Name.Last()}")
                .With(x => x.ContactEmail = $"Automation_{Faker.Internet.Email()}")
                .With(x => x.TelephoneNumber = Faker.Phone.Number())
                .With(x => x.Username = $"Automation_{Faker.Internet.Email()}")
                .With(x => x.DisplayName = $"Automation_{Faker.Name.FullName()}")
                .With(x => x.OrganisationName = $"{Faker.Company.Name()}")
                .Build().ToList();

                participants[0].CaseRoleName = "Claimant";
                participants[0].HearingRoleName = "Litigant in person";
                participants[0].Representee = null;

                participants[1].CaseRoleName = "Claimant";
                participants[1].HearingRoleName = "Representative";
                participants[1].Representee = participants[0].DisplayName;

                participants[2].CaseRoleName = "Defendant";
                participants[2].HearingRoleName = "Litigant in person";
                participants[2].Representee = null;

                participants[3].CaseRoleName = "Defendant";
                participants[3].HearingRoleName = "Representative";
                participants[3].Representee = participants[2].DisplayName;

                participants[4].CaseRoleName = "Judge";
                participants[4].HearingRoleName = "Judge";
                participants[4].Representee = null;

                return participants;
            }
        }

        private List<CaseRequest> Cases
        {
            get
            {
                var cases = Builder<CaseRequest>.CreateListOfSize(1).Build().ToList();
                cases[0].IsLeadCase = false;
                cases[0].Name = $"Bookings Api Automated Test {Faker.RandomNumber.Next(0, 9999999)}";
                cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";

                return cases;
            }
        }
        
        const string createdBy = "caseAdmin@emailaddress.com";

        private BookNewHearingRequest request => Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Civil Money Claims")
                .With(x => x.HearingTypeName = "Application to Set Judgment Aside")
                .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
                .With(x => x.ScheduledDateTime = DateTime.Today.ToUniversalTime().AddDays(1).AddMinutes(-1))
                .With(x => x.ScheduledDuration = 5)
                .With(x => x.Participants = Participants)
                .With(x => x.Cases = Cases)
                .With(x => x.CreatedBy = createdBy)
                .With(x => x.QuestionnaireNotRequired = false)
                .With(x => x.Endpoints = new List<EndpointRequest> {new EndpointRequest{DisplayName = "Cool endpoint 1"}})
                .Build();

        private List<CaseRole> CaseRoles => new List<CaseRole> 
        {
            CreateCaseAndHearingRoles(1, "Claimant",new List<string>{ "Litigant in person", "Representative"}),
            CreateCaseAndHearingRoles(2, "Defendant",new List<string>{ "Litigant in person", "Representative"}),
            CreateCaseAndHearingRoles(3, "Judge", new List<string>{ "Judge"})
        };

        private CaseRole CreateCaseAndHearingRoles(int caseId, string caseRoleName, List<string> roles)
        {
            var hearingRoles = new List<HearingRole>();
            
            foreach (var role in roles)
            {
                hearingRoles.Add(new HearingRole(1, role) { UserRole = new UserRole(1, "TestUser") });
            }

            var caseRole = new CaseRole(caseId, caseRoleName) { HearingRoles = hearingRoles };

            return caseRole;
        }


        [SetUp]
        public void TestInitialize()
        {
            var caseType = new CaseType(1, "Civil")
            {
                CaseRoles = CaseRoles,
                HearingTypes = new List<HearingType> { new HearingType("Application to Set Judgment Aside") }
            };

            QueryHandlerMock
            .Setup(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
            .ReturnsAsync(caseType);

            QueryHandlerMock
            .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
            .ReturnsAsync(new List<HearingVenue> { new HearingVenue(1, "Birmingham Civil and Family Justice Centre") });

            var hearing = GetHearing("123");

            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
             .ReturnsAsync(hearing);
        }


        [Test]
        public async Task Should_successfully_book_new_hearing()
        { 
            var response = await Controller.BookNewHearing(request);

            response.Should().NotBeNull();
            var result = (CreatedAtActionResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            QueryHandlerMock.Verify(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()), Times.Once);

            QueryHandlerMock.Verify(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()), Times.Once);

            RandomGenerator.Verify(x => x.GetWeakDeterministic(It.IsAny<long>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
            
            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<CreateVideoHearingCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_badrequest_without_matching_casetype()
        {
            QueryHandlerMock
           .Setup(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
           .ReturnsAsync((CaseType)null);

            var result = await Controller.BookNewHearing(request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.CaseTypeName), "Case type does not exist");
        }

        [Test]
        public async Task Should_return_badrequest_without_matching_hearingtype()
        {
            var caseType = new CaseType(1, "Civil")
            {
                CaseRoles = CaseRoles,
                HearingTypes = new List<HearingType> { new HearingType("Not matching") }
            };

            QueryHandlerMock
            .Setup(x => x.Handle<GetCaseTypeQuery, CaseType>(It.IsAny<GetCaseTypeQuery>()))
            .ReturnsAsync(caseType);

            var result = await Controller.BookNewHearing(request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.HearingTypeName), "Hearing type does not exist");
        }

        [Test]
        public async Task Should_return_badrequest_without_matching_hearingvenue()
        {
            QueryHandlerMock
           .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
           .ReturnsAsync(new List<HearingVenue> { new HearingVenue(1, "Not matching") });

            var result = await Controller.BookNewHearing(request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.HearingVenueName), "Hearing venue does not exist");
        }
    }
}
