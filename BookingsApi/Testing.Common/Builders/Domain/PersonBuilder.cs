using FizzWare.NBuilder;
using BookingsApi.Domain;
using Bogus;
using Person = BookingsApi.Domain.Person;

namespace Testing.Common.Builders.Domain
{
    public class PersonBuilder
    {
        private readonly Person _person;
        private readonly BuilderSettings _settings;
        private Faker _faker = new Faker();

        public PersonBuilder(bool ignoreId = false,bool treatPersonAsNew = true)
        {
            _settings = new BuilderSettings();
            if (ignoreId)
            {
                _settings.DisablePropertyNamingFor<Organisation, long>(x => x.Id);
                
            }
            _person = new Builder(_settings).CreateNew<Person>().WithFactory(() =>
                {
                    var contactEmail = $"vh_automation_{_faker.Random.Number()}@hmcts.net";
                    var person = new Person(
                        _faker.Name.Prefix(),
                        "VH Automation_FirstName",
                        "VH Automation_LastName",
                        contactEmail:contactEmail,
                        username:contactEmail
                        );
                    if (treatPersonAsNew) return person;
                    var username = $"vh_automation_{_faker.Random.Number()}@hearings.reform.hmcts.net";
                    person.UpdateUsername(username);

                    return person;
                })
                .With(x => x.TelephoneNumber, "01234567890")
                .Build();
        }

        public PersonBuilder(string userId, string contactEmail)
        {
            var settings = new BuilderSettings();
            _person = new Builder(settings).CreateNew<Person>().WithFactory(() =>
                    new Person(_faker.Name.Prefix(), $"Automation_FirstName", $"Automation_LastName", contactEmail, userId))
                .Build();
        }

        public PersonBuilder(string contactEmail)
        {
            var settings = new BuilderSettings();
            _person = new Builder(settings).CreateNew<Person>().WithFactory(() =>
                    new Person(_faker.Name.Prefix(), 
                        $"Automation_FirstName", 
                        $"Automation_LastName", 
                        contactEmail, 
                        $"Automation_{_faker.Random.Number()}@hearings.reform.hmcts.net"))
                .Build();
        }

        public PersonBuilder WithOrganisation()
        {
            _person.Organisation = new Builder(_settings).CreateNew<Organisation>()
                .WithFactory(() => new Organisation())
                .With(x => x.Name = _faker.Company.CompanyName()).Build();
            return this;
        }

        public Person Build()
        {
            return _person;
        }
    }
}