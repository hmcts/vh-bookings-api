using System;
using Faker;
using FizzWare.NBuilder;
using BookingsApi.Domain;

namespace Testing.Common.Builders.Domain
{
    public class JudiciaryPersonBuilder
    {
        private readonly JudiciaryPerson _judiciaryPerson;

        public JudiciaryPersonBuilder(Guid? externalRefId = null)
        {
            var settings = new BuilderSettings();
            _judiciaryPerson = new Builder(settings).CreateNew<JudiciaryPerson>().WithFactory(() =>
                    new JudiciaryPerson(
                        externalRefId ?? Guid.NewGuid(),
                        $"{RandomNumber.Next(0, 1000)}",
                        Name.Prefix(),
                        $"Automation_{Name.First()}",
                        $"Automation_{Name.Last()}",
                        Name.FullName(),
                        $"{RandomNumber.Next(1000, 100000)}",
                        $"Automation_{RandomNumber.Next()}@hmcts.net"
                        , false))
                .Build();
        }

        public JudiciaryPerson Build()
        {
            return _judiciaryPerson;
        }
    }
}