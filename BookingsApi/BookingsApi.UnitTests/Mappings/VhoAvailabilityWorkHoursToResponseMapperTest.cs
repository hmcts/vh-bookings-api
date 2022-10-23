using System;
using System.Collections.Generic;
using BookingsApi.Contract.Responses;
using BookingsApi.Domain;
using BookingsApi.Mappings;
using BookingsApi.UnitTests.Utilities;
using Newtonsoft.Json;
using NUnit.Framework;
using DayOfWeek = BookingsApi.Domain.DayOfWeek;

namespace BookingsApi.UnitTests.Mappings
{
    public class VhoAvailabilityWorkHoursToResponseMapperTest : TestBase
    {
        [Test]
        public void Map_Availability_VhoNonAvailabilityWorkHoursToResponseMapper()
        {
            // Arrange
            var start = TimeSpan.Zero;
            var end = TimeSpan.Zero;
            var vhoWorkHours = new List<VhoWorkHours>
            {
                new()
                {
                    StartTime = start,
                    EndTime = end,
                    CreatedBy = "created.by@email.com",
                    JusticeUserId = Guid.NewGuid(),
                    DayOfWeekId = 1,
                    DayOfWeek = new DayOfWeek {Day = "Monday"}
                }
            };

            var vhoWorkHoursResponse = new List<VhoWorkHoursResponse>()
            {
                new VhoWorkHoursResponse
                {
                    StartTime = start,
                    EndTime = end,
                    DayOfWeekId = 1,
                    DayOfWeek = "Monday"
                }
            };

            var expectedJson = JsonConvert.SerializeObject(vhoWorkHoursResponse);

            // Act
            var result = VhoWorkHoursToResponseMapper.Map(vhoWorkHours);
            var actualVhoNonAvailabilityResponseJson = JsonConvert.SerializeObject(result);

            // Assert
            Assert.AreEqual(expectedJson, actualVhoNonAvailabilityResponseJson);
        }
    }
}
