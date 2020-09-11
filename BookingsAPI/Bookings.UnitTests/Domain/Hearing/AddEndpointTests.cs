using Bookings.Domain;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Hearing
{
    public class AddEndpointTests
    {
        [Test]
        public void Should_add_new_endpoint()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeAddCount = hearing.GetEndpoints().Count;
            hearing.AddEndpoint(new Bookings.Domain.Endpoint("DisplayName", "sip@address.com", "1111") );
            var afterAddCount = hearing.GetEndpoints().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
        [Test]
        public void Should_not_add_endpoint_with_same_sip_address()
        {
            var hearing = new VideoHearingBuilder().Build();
            var epList = new System.Collections.Generic.List<Endpoint>();
            var endpoint = new Endpoint("DisplayName", "sip@address.com", "1111");
            epList.Add(endpoint);

            endpoint = new Endpoint("DisplayName1", "sip@address.com", "2222");
            epList.Add(endpoint);

            Action action = () => hearing.AddEndpoints(epList);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == $"Endpoint already exists in hearing").Should().BeTrue();
        }

        [Test]
        public void Should_add_endpoint_with_unique_sip_address()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeAddCount = hearing.GetEndpoints().Count;
            var epList = new System.Collections.Generic.List<Endpoint>();
            var endpoint = new Endpoint("DisplayName", "sip@address.com", "1111");
            epList.Add(endpoint);

            endpoint = new Endpoint("DisplayName1", "sip2@address.com", "2222");
            epList.Add(endpoint);

            hearing.AddEndpoints(epList);
            var afterAddCount = hearing.GetEndpoints().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }

        [Test]
        public void Should_add_new_endpoint_with_defence_advocate()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeAddCount = hearing.GetEndpoints().Count;
            var endpoint = new Endpoint("DisplayName", "sip@address.com", "1111");

            var person = new Person("Mr.", "FirstName", "LastName", "firstName.lastName@email.com");
            var defenceAdvocate = hearing.AddRepresentative(person,
                new HearingRole(1, "HearingRoleName"), new CaseRole(1, "CaseRoleName"), "displayName", "reference001", "individual");
            endpoint.AssignDefenceAdvocate(defenceAdvocate);
            hearing.AddEndpoint(endpoint);
            var afterAddCount = hearing.GetEndpoints().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }

        [Test]
        public void Should_not_add_new_endpoint_with_same_defence_advocate()
        {
            var hearing = new VideoHearingBuilder().Build();
            var epList = new System.Collections.Generic.List<Endpoint>();
            var endpoint = new Endpoint("DisplayName", "sip@address.com", "1111");
            var person = new Person("Mr.", "FirstName", "LastName", "firstName.lastName@email.com");
            var defenceAdvocate = hearing.AddRepresentative(person,
                new HearingRole(1, "HearingRoleName"), new CaseRole(1, "CaseRoleName"), "displayName", "reference001", "individual");
            endpoint.AssignDefenceAdvocate(defenceAdvocate);
            epList.Add(endpoint);

            endpoint = new Endpoint("DisplayName1", "sip@address1.com", "2222");
            endpoint.AssignDefenceAdvocate(defenceAdvocate);
            epList.Add(endpoint);

            Action action = () => hearing.AddEndpoints(epList);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == $"Defence advocate already exists for this endpoint").Should().BeTrue();

            var afterAddCount = hearing.GetEndpoints().Count;
            afterAddCount.Should().Be(1);
        }

        [Test]
        public void Should_add_new_endpoint_with_unique_defence_advocate()
        {
            var hearing = new VideoHearingBuilder().Build();
            var beforeAddCount = hearing.GetEndpoints().Count;
            var epList = new System.Collections.Generic.List<Endpoint>();
            var endpoint = new Endpoint("DisplayName", "sip@address.com", "1111");
            var person = new Person("Mr.", "FirstName", "LastName", "firstName.lastName@email.com");
            var defenceAdvocate = hearing.AddRepresentative(person,
                new HearingRole(1, "HearingRoleName"), new CaseRole(1, "CaseRoleName"), "displayName", "reference001", "individual");
            endpoint.AssignDefenceAdvocate(defenceAdvocate);
            epList.Add(endpoint);

            endpoint = new Endpoint("DisplayName1", "sip@address1.com", "2222");
            person = new Person("Mr.", "FirstName1", "LastName1", "firstName1.lastName1@email.com");
            defenceAdvocate = hearing.AddRepresentative(person,
                new HearingRole(1, "HearingRoleName"), new CaseRole(1, "CaseRoleName"), "displayName1", "reference002", "individual001");
            endpoint.AssignDefenceAdvocate(defenceAdvocate);
            epList.Add(endpoint);

            hearing.AddEndpoints(epList);
            var afterAddCount = hearing.GetEndpoints().Count;
            afterAddCount.Should().BeGreaterThan(beforeAddCount);
        }
    }
}


