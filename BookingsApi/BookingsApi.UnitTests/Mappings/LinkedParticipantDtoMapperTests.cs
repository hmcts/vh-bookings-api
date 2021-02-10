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
            dto.Type.Should().BeEquivalentTo(_linkedParticipant.Type);
        }
        
        [Test]
        public void Should_Return_An_Empty_List_When_Request_Is_Null()
        {
            var dto = LinkedParticipantDtoMapper.MapToDto(null);

            dto.Should().NotBeNull();
            dto.Should().BeOfType<LinkedParticipantDto>();
        }
        
        private LinkedParticipant BuildLinkedParticipant()
        {
            return new LinkedParticipant(Guid.NewGuid(), Guid.NewGuid(), LinkedParticipantType.Interpreter);
        }
    }
}