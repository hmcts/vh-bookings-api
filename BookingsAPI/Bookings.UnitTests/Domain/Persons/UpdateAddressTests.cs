using Bookings.Domain;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Domain.Persons
{
    public class UpdateAddressTests
    {
        [Test]
        public void should_update_address()
        {
            var person = new PersonBuilder().Build();
            var beforeUpdatedDate = person.UpdatedDate;
            person.Address.Should().BeNull();
            var address = Builder<Address>.CreateNew().Build();
            person.UpdateAddress(address);
            person.Address.Should().BeEquivalentTo(address);
            person.UpdatedDate.Should().BeAfter(beforeUpdatedDate);
        }
    }
}