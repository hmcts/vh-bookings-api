using System;
using System.Collections.Generic;
using BookingsApi.Domain;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Domain.JusticeUser;

public class IsDuringNonAvailableHoursTests
{
    [Test]
    public void should_return_true_when_hearing_is_during_non_working_hours()
    {
        // arrange
        var nonWorkHoursStartTime = DateTime.Today.AddHours(8);
        var nonWorkHoursEndTime = DateTime.Today.AddHours(11);
        
        var hearingStartTime = DateTime.Today.AddHours(10);
        var hearingEndTime = hearingStartTime.AddMinutes(45);
        
        var nonAvailability = new VhoNonAvailability()
        {
            StartTime = nonWorkHoursStartTime,
            EndTime = nonWorkHoursEndTime,
            Deleted = false
        };
        
        var justiceUser = Builder<BookingsApi.Domain.JusticeUser>.CreateNew()
            .With(x => x.VhoNonAvailability, new List<VhoNonAvailability> {nonAvailability})
            .Build();
        
        // act
        var result = justiceUser.IsDuringNonAvailableHours(hearingStartTime, hearingEndTime);

        // assert
        result.Should().BeTrue();
    }
    
    [Test]
    public void should_return_false_when_hearing_is_not_during_non_working_hours()
    {
        // arrange
        var nonWorkHoursStartTime = DateTime.Today.AddHours(13);
        var nonWorkHoursEndTime = DateTime.Today.AddHours(18);
        
        var hearingStartTime = DateTime.Today.AddHours(10);
        var hearingEndTime = hearingStartTime.AddMinutes(45);
        
        var nonAvailability = new VhoNonAvailability()
        {
            StartTime = nonWorkHoursStartTime,
            EndTime = nonWorkHoursEndTime,
            Deleted = false
        };
        
        var justiceUser = Builder<BookingsApi.Domain.JusticeUser>.CreateNew()
            .With(x => x.VhoNonAvailability, new List<VhoNonAvailability> {nonAvailability})
            .Build();
        
        // act
        var result = justiceUser.IsDuringNonAvailableHours(hearingStartTime, hearingEndTime);

        // assert
        result.Should().BeFalse();
    }
}