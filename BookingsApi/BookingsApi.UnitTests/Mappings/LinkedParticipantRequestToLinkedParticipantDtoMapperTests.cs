using System.Collections.Generic;
using BookingsApi.Contract.Enums;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Dtos;
using BookingsApi.Mappings;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Mappings
{
    public class LinkedParticipantRequestToLinkedParticipantDtoMapperTests
    {
        private List<LinkedParticipantRequest> _requests;

        [SetUp]
        public void SetUp()
        {
            _requests = new List<LinkedParticipantRequest>();
            
            var request = BuildRequest();
            _requests.Add(request);
        }
        
        [Test]
        public void Should_Map_A_Single_LinkedParticipantRequest_To_Dto_Successfully()
        {
            var dto = LinkedParticipantRequestToLinkedParticipantDtoMapper.MapToDto(_requests);

            dto.Should().BeOfType<List<LinkedParticipantDto>>();
            dto.Count.Should().Be(1);
        }
        
        [Test]
        public void Should_Map_Multiple_LinkedParticipantRequests_To_Dto_Successfully()
        {
            var secondRequest = new LinkedParticipantRequest
            {
                ParticipantContactEmail = "second@hmcts.net",
                LinkedParticipantContactEmail = "twolink@hmcts.net",
                Type = LinkedParticipantType.Interpreter
            };
            _requests.Add(secondRequest);
            
            var dtos = LinkedParticipantRequestToLinkedParticipantDtoMapper.MapToDto(_requests);

            dtos.Count.Should().Be(2);
        }
        
        [Test]
        public void Should_Return_An_Empty_List_When_Request_Is_Null()
        {
            var dto = LinkedParticipantRequestToLinkedParticipantDtoMapper.MapToDto(null);

            dto.Should().NotBeNull();
            dto.Count.Should().Be(0);
        }
        
        private LinkedParticipantRequest BuildRequest()
        {
            return new LinkedParticipantRequest
            {
                ParticipantContactEmail = "test@hmcts.net",
                LinkedParticipantContactEmail = "link@hmcts.net",
                Type = LinkedParticipantType.Interpreter
            };
        }
    }
}