using System.Collections.Generic;
using BookingsApi.DAL.Dtos;
using BookingsApi.Mappings.V1;

namespace BookingsApi.UnitTests.Mappings.V1
{
    public class AnonymiseDataResponseMapperTests
    {
        [Test]
        public void Maps_AnonymiseDataDto_To_AnonymiseDataResponse()
        {
            var anonymiseDataDto = new AnonymisationDataDto
                {Usernames = new List<string>(), HearingIds = new List<Guid>()};

            var result = AnonymisationDataResponseMapper.Map(anonymiseDataDto);

            result.Usernames.Should().BeSameAs(anonymiseDataDto.Usernames);
            result.HearingIds.Should().BeSameAs(anonymiseDataDto.HearingIds);
        }
        
    }
}