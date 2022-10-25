using System;
using System.Collections.Generic;
using BookingsApi.Contract.Responses;
using BookingsApi.Domain;
using BookingsApi.Mappings;
using BookingsApi.UnitTests.Utilities;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Mappings
{
    public class VhoNonAvailabilityWorkHoursToResponseMapperTest : TestBase
    {
        [Test]
        public void Map_NonAvailability_VhoNonAvailabilityWorkHoursToResponseMapper()
        {
            // Arrange
            var start = DateTime.Now;
            var end = DateTime.Now;
            var vhoNonAvailability = new List<VhoNonAvailability>
            {
                new VhoNonAvailability()
                {
                    StartTime = start, 
                    EndTime = end,
                    CreatedBy = "created.by@email.com",
                    JusticeUserId = Guid.NewGuid()
                }
            };

            var vhoNonAvailabilityWorkHoursResponse = new List<VhoNonAvailabilityWorkHoursResponse>()
            {
                new VhoNonAvailabilityWorkHoursResponse()
                {
                    StartTime = start, 
                    EndTime = end
                }
            };

            var expectedNonAvailabilityJson = JsonConvert.SerializeObject(vhoNonAvailabilityWorkHoursResponse);

            // Act
            var result = VhoNonAvailabilityWorkHoursResponseMapper.Map(vhoNonAvailability);
            var actualVhoNonAvailabilityResponseJson = JsonConvert.SerializeObject(result);

            // Assert
            Assert.AreEqual(expectedNonAvailabilityJson, actualVhoNonAvailabilityResponseJson);
        }
    }
}
