﻿using System;
using Faker;
using FizzWare.NBuilder;
using BookingsApi.Domain;

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
                _settings.DisablePropertyNamingFor<Organisation, long>(x => x.Id);
                
            }
            _person = new Builder(_settings).CreateNew<Person>().WithFactory(() =>
                    new Person(Name.Prefix(), $"Automation_{Name.First()}", $"Automation_{Name.Last()}", $"Automation_{RandomNumber.Next()}@hmcts.net", $"Automation_{RandomNumber.Next()}@hmcts.net"))
                .With(x => x.UpdatedDate, DateTime.MinValue)
                .With(x => x.TelephoneNumber, "(+44)123 456")
                .Build();
        }

        public PersonBuilder(string userId, string contactEmail)
        {
            var settings = new BuilderSettings();
            _person = new Builder(settings).CreateNew<Person>().WithFactory(() =>
                    new Person(Name.Prefix(), $"Automation_{Name.First()}", $"Automation_{Name.Last()}", contactEmail, userId))
                .With(x => x.TelephoneNumber, "(+44)123 456")
                .Build();
        }

        public PersonBuilder WithOrganisation()
        {
            _person.Organisation = new Builder(_settings).CreateNew<Organisation>()
                .WithFactory(() => new Organisation())
                .With(x => x.Name = Company.Name())
                .With(x => "(+44)123 456")
                .Build();
            return this;
        }

        public Person Build()
        {
            return _person;
        }
    }
}