using System;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services;
using BookingsApi.Infrastructure.Services.Dtos;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Mappings
{
    public class LinkedParticipantDtoMapperTests
    {
        private LinkedParticipant _linkedParticipant;
        
        [SetUp]
        public void SetUp()
        {
            _linkedParticipant = BuildLinkedParticipant();
        }
        
        [Test]
        public void Should_Map_LinkedParticipant_To_Dto_Successfully()
        {
            var dto = LinkedParticipantDtoMapper.MapToDto(_linkedParticipant);

            dto.ParticipantId.Should().Be(_linkedParticipant.ParticipantId);
            dto.LinkedId.Should().Be(_linkedParticipant.LinkedId);
            dto.Type.Should().Be(_linkedParticipant.Type);
        }

        private LinkedParticipant BuildLinkedParticipant()
        {
            return new LinkedParticipant(Guid.NewGuid(), Guid.NewGuid(), LinkedParticipantType.Interpreter);
        }
    }
}