using System;
using Faker;
using FizzWare.NBuilder;
using Bookings.Domain;
using Address = Bookings.Domain.Address;

namespace Testing.Common.Builders.Domain
{
    public class PersonBuilder
    {
        private readonly Person _person;
        private readonly BuilderSettings _settings;

        public PersonBuilder(bool ignoreId = false)
        {
            _settings = new BuilderSettings();
            if (ignoreId)
            {
                _settings.DisablePropertyNamingFor<Address, long>(x => x.Id);
                _settings.DisablePropertyNamingFor<Organisation, long>(x => x.Id);
                
            }
            _person = new Builder(_settings).CreateNew<Person>().WithFactory(() =>
                    new Person(Name.Prefix(), $"Automation_{Name.First()}", $"Automation_{Name.Last()}", $"Automation_{Internet.Email()}")).With(x => x.ContactEmail = $"Automation_{Internet.Email()}")
                .With(x => x.UpdatedDate, DateTime.MinValue)
                .Build();
        }

        public PersonBuilder(string userId)
        {
            var settings = new BuilderSettings();
            _person = new Builder(settings).CreateNew<Person>().WithFactory(() =>
                    new Person(Name.Prefix(), $"Automation_{Name.First()}", $"Automation_{Name.Last()}", userId)).With(x => x.ContactEmail = $"Automation_{Internet.Email()}")
                .Build();
        }

        public PersonBuilder WithAddress()
        {
           var address = new Builder(_settings).CreateNew<Address>()
                .WithFactory(() => new Address())
                .With(x => x.Street = Faker.Address.StreetAddress())
                .With(x => x.Postcode = Faker.Address.UkPostCode())
                .With(x => x.City = Faker.Address.City())
                .With(x => x.County = Faker.Address.Country())
                .With(x => x.HouseNumber = Faker.Address.StreetAddress())
                .Build();
           _person.UpdateAddress(address);
            return this;
        }
        
        public PersonBuilder WithOrganisation()
        {
            _person.Organisation = new Builder(_settings).CreateNew<Organisation>()
                .WithFactory(() => new Organisation())
                .With(x => x.Name = Company.Name()).Build();
            return this;
        }

        public Person Build()
        {
            return _person;
        }
    }
}