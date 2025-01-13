using System;
using FizzWare.NBuilder;
using BookingsApi.Domain;
using Bogus;

namespace Testing.Common.Builders.Domain
{
    public class JudiciaryPersonBuilder
    {
        private readonly JudiciaryPerson _judiciaryPerson;
        private Faker _faker = new Faker();

        public JudiciaryPersonBuilder(string personalCode = null)
        {
            var settings = new BuilderSettings();
            _judiciaryPerson = new Builder(settings).CreateNew<JudiciaryPerson>().WithFactory(() =>
                    new JudiciaryPerson(
                        Guid.NewGuid().ToString(),
                        personalCode,
                        _faker.Name.Prefix(),
                        $"Automation_FirstName",
                        $"Automation_LastName",
                        _faker.Name.FullName(),
                        $"{_faker.Random.Number(1000, 100000)}",
                        $"Automation_{_faker.Random.Number(0, 9999999)}@hmcts.net", 
                        _faker.Phone.PhoneNumber(),
                        false,
                        false,
                        string.Empty))
                .Build();
        }

        public JudiciaryPerson Build()
        {
            return _judiciaryPerson;
        }
    }
}