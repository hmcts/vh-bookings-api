using System;
using Faker;
using FizzWare.NBuilder;
using BookingsApi.Domain;

namespace Testing.Common.Builders.Domain
{
    public class JudiciaryPersonBuilder
    {
        private readonly JudiciaryPerson _judiciaryPerson;

        public JudiciaryPersonBuilder(string personalCode = null)
        {
            var settings = new BuilderSettings();
            _judiciaryPerson = new Builder(settings).CreateNew<JudiciaryPerson>().WithFactory(() =>
                    new JudiciaryPerson(
                        Guid.NewGuid().ToString(),
                        personalCode,
                        Name.Prefix(),
                        $"Automation_FirstName",
                        $"Automation_LastName",
                        Name.FullName(),
                        $"{RandomNumber.Next(1000, 100000)}",
                        $"Automation_{RandomNumber.Next()}@hmcts.net", 
                        Phone.Number(),
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