using System;
using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Domain.Configuration;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Domain.JusticeUser;

public class IsDateBetweenWorkingHoursTests
{
    [Test]
    public void should_return_false_when_no_working_hours_are_set()
    {
        // arrange
        var config = new AllocateHearingConfiguration {AllowHearingToEndAfterWorkEndTime = false};
        var hearingStartTime = new DateTime(DateTime.Today.Year + 1, 3, 1, 10, 0, 0, DateTimeKind.Utc);
        var hearingEndTime = hearingStartTime.AddMinutes(45);
        
        var justiceUser = Builder<BookingsApi.Domain.JusticeUser>.CreateNew()
            .With(x => x.VhoWorkHours, new List<VhoWorkHours>())
            .Build();
        
        // act
        var result = justiceUser.IsDateBetweenWorkingHours(hearingStartTime, hearingEndTime, config);
        
        // assert
        result.Should().BeFalse();
    }
    
    [Test]
    public void should_return_true_when_start_and_end_time_is_between_working_hours()
    {
        // arrange
        var config = new AllocateHearingConfiguration {AllowHearingToEndAfterWorkEndTime = false};
        var workHoursStartTime = new TimeSpan(9, 0, 0);
        var workHoursEndTime = new TimeSpan(11, 0, 0);
        
        var hearingStartTime = new DateTime(DateTime.Today.Year + 1, 3, 1, 10, 0, 0, DateTimeKind.Utc);
        var hearingEndTime = hearingStartTime.AddMinutes(45);
        
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new BookingsApi.Domain.DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime,
            EndTime = workHoursEndTime
        };
        var justiceUser = Builder<BookingsApi.Domain.JusticeUser>.CreateNew()
            .With(x => x.VhoWorkHours, new List<VhoWorkHours> {workHours})
            .Build();
        
        // act
        var result = justiceUser.IsDateBetweenWorkingHours(hearingStartTime, hearingEndTime, config);
        
        // assert
        result.Should().BeTrue();
    }
    
    [Test]
    public void should_return_false_when_start_is_before_working_hours()
    {
        // arrange
        var config = new AllocateHearingConfiguration {AllowHearingToEndAfterWorkEndTime = false};
        var workHoursStartTime = new TimeSpan(9, 0, 0);
        var workHoursEndTime = new TimeSpan(11, 0, 0);
        
        var hearingStartTime = new DateTime(DateTime.Today.Year + 1, 3, 1, workHoursStartTime.Hours - 3, 0, 0, DateTimeKind.Utc);
        var hearingEndTime = new DateTime(DateTime.Today.Year + 1, 3, 1, 10, 50, 0, DateTimeKind.Utc);
        
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new BookingsApi.Domain.DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime,
            EndTime = workHoursEndTime
        };
        var justiceUser = Builder<BookingsApi.Domain.JusticeUser>.CreateNew()
            .With(x => x.VhoWorkHours, new List<VhoWorkHours> {workHours})
            .Build();
        
        // act
        var result = justiceUser.IsDateBetweenWorkingHours(hearingStartTime, hearingEndTime, config);
        
        // assert
        result.Should().BeFalse();
    }
    
    [Test]
    public void should_return_true_when_start_is_before_working_hours_but_override_is_true()
    {
        // arrange
        var config = new AllocateHearingConfiguration {AllowHearingToStartBeforeWorkStartTime = true};
        var workHoursStartTime = new TimeSpan(9, 0, 0);
        var workHoursEndTime = new TimeSpan(11, 0,0);
        
        var hearingStartTime = new DateTime(DateTime.Today.Year + 1, 3, 1, workHoursStartTime.Hours - 3, 0, 0, DateTimeKind.Utc);
        var hearingEndTime = new DateTime(DateTime.Today.Year + 1, 3, 1, 10, 50, 0, DateTimeKind.Utc);
        
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new BookingsApi.Domain.DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime,
            EndTime = workHoursEndTime
        };
        var justiceUser = Builder<BookingsApi.Domain.JusticeUser>.CreateNew()
            .With(x => x.VhoWorkHours, new List<VhoWorkHours> {workHours})
            .Build();
        
        // act
        var result = justiceUser.IsDateBetweenWorkingHours(hearingStartTime, hearingEndTime, config);
        
        // assert
        result.Should().BeTrue();
    }
    
    [Test]
    public void should_return_false_when_end_time_exceeds_work_hours()
    {
        // arrange
        var config = new AllocateHearingConfiguration {AllowHearingToEndAfterWorkEndTime = false};
        var workHoursStartTime = new TimeSpan(9, 30, 0);
        var workHoursEndTime = new TimeSpan(12, 30, 0);
        
        var hearingStartTime = new DateTime(DateTime.Today.Year + 1, 3, 1, 11, 0, 0, DateTimeKind.Utc);
        var hearingEndTime = hearingStartTime.AddMinutes(480);
        
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new BookingsApi.Domain.DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime,
            EndTime = workHoursEndTime
        };
        var justiceUser = Builder<BookingsApi.Domain.JusticeUser>.CreateNew()
            .With(x => x.VhoWorkHours, new List<VhoWorkHours> {workHours})
            .Build();
        
        // act
        var result = justiceUser.IsDateBetweenWorkingHours(hearingStartTime, hearingEndTime, config);
        
        // assert
        result.Should().BeFalse();
    }
    
    [Test]
    public void should_return_true_when_end_time_exceeds_work_hours_but_override_is_true()
    {
        // arrange
        var config = new AllocateHearingConfiguration {AllowHearingToEndAfterWorkEndTime = true};
        var workHoursStartTime = new TimeSpan(9, 0, 0);
        var workHoursEndTime = new TimeSpan(11, 0, 0);
        
        var hearingStartTime = new DateTime(DateTime.Today.Year + 1, 3, 1, 10, 0, 0, DateTimeKind.Utc);
        var hearingEndTime = hearingStartTime.AddMinutes(600);
        
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new BookingsApi.Domain.DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime,
            EndTime = workHoursEndTime
        };
        var justiceUser = Builder<BookingsApi.Domain.JusticeUser>.CreateNew()
            .With(x => x.VhoWorkHours, new List<VhoWorkHours> {workHours})
            .Build();
        
        // act
        var result = justiceUser.IsDateBetweenWorkingHours(hearingStartTime, hearingEndTime, config);
        
        // assert
        result.Should().BeTrue();
    }
}