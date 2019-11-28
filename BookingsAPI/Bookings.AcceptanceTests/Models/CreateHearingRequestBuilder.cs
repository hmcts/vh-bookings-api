using System;
using System.Linq;
using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Requests;
using FizzWare.NBuilder;

namespace Bookings.AcceptanceTests.Models
{
    internal class CreateHearingRequestBuilder
    {
        private TestContext _context;
        private readonly BookNewHearingRequest _request;

        public CreateHearingRequestBuilder()
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
                .With(x => x.HouseNumber = $"{Faker.RandomNumber.Next(0, 999)}")
                .With(x => x.Street = $"{Faker.Address.StreetName()}")
                .With(x => x.Postcode = $"{Faker.Address.UkPostCode()}")
                .With(x => x.City = $"{Faker.Address.City()}")
                .With(x => x.County = $"{Faker.Address.UkCounty()}")
                .Build().ToList();

            participants[0].CaseRoleName = "Claimant";
            participants[0].HearingRoleName = "Claimant LIP";
            participants[0].SolicitorsReference = participants[1].DisplayName;
            participants[0].Representee = null;

            participants[1].CaseRoleName = "Claimant";
            participants[1].HearingRoleName = "Solicitor";
            participants[1].SolicitorsReference = null;
            participants[1].Representee = participants[0].DisplayName;

            participants[2].CaseRoleName = "Defendant";
            participants[2].HearingRoleName = "Defendant LIP";
            participants[2].SolicitorsReference = participants[3].DisplayName;
            participants[2].Representee = null;

            participants[3].CaseRoleName = "Defendant";
            participants[3].HearingRoleName = "Solicitor";
            participants[2].SolicitorsReference = null;
            participants[3].Representee = participants[2].DisplayName;

            participants[4].CaseRoleName = "Judge";
            participants[4].HearingRoleName = "Judge";
            participants[4].SolicitorsReference = null;
            participants[4].Representee = null;

            var cases = Builder<CaseRequest>.CreateListOfSize(1).Build().ToList();
            cases[0].IsLeadCase = false;
            cases[0].Name = $"Bookings Api Automated Test {Faker.RandomNumber.Next(0, 9999999)}";
            cases[0].Number = $"{Faker.RandomNumber.Next(0, 9999)}/{Faker.RandomNumber.Next(0, 9999)}";

            const string createdBy = "caseAdmin@emailaddress.com";

            _request = Builder<BookNewHearingRequest>.CreateNew()
                .With(x => x.CaseTypeName = "Civil Money Claims")
                .With(x => x.HearingTypeName = "Application to Set Judgment Aside")
                .With(x => x.HearingVenueName = "Birmingham Civil and Family Justice Centre")
                .With(x => x.ScheduledDateTime = DateTime.Today.ToUniversalTime().AddDays(1).AddMinutes(-1))
                .With(x => x.ScheduledDuration = 5)
                .With(x => x.Participants = participants)
                .With(x => x.Cases = cases)
                .With(x => x.CreatedBy = createdBy)
                .With(x => x.QuestionnaireNotRequired = false)
                .Build();
        }

        public CreateHearingRequestBuilder WithContext(TestContext context)
        {
            _context = context;
            return this;
        }

        public BookNewHearingRequest Build()
        {
            _context.HearingRequest = _request;
            return _request;
        }
    }
}
