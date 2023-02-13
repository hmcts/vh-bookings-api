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
        var hearingStartTime = DateTime.Today.AddHours(10);
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
        var workHoursStartTime = DateTime.Today.AddHours(9);
        var workHoursEndTime = DateTime.Today.AddHours(11);
        
        var hearingStartTime = DateTime.Today.AddHours(10);
        var hearingEndTime = hearingStartTime.AddMinutes(45);
        
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new BookingsApi.Domain.DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime.TimeOfDay,
            EndTime = workHoursEndTime.TimeOfDay
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
        var workHoursStartTime = DateTime.Today.AddHours(9);
        var workHoursEndTime = DateTime.Today.AddHours(11);
        
        var hearingStartTime = workHoursStartTime.AddHours(-3);
        var hearingEndTime = workHoursEndTime.AddMinutes(-10);
        
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new BookingsApi.Domain.DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime.TimeOfDay,
            EndTime = workHoursEndTime.TimeOfDay
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
        var workHoursStartTime = DateTime.Today.AddHours(9);
        var workHoursEndTime = DateTime.Today.AddHours(11);
        
        var hearingStartTime = workHoursStartTime.AddHours(-3);
        var hearingEndTime = workHoursEndTime.AddMinutes(-10);
        
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new BookingsApi.Domain.DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime.TimeOfDay,
            EndTime = workHoursEndTime.TimeOfDay
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
        var workHoursStartTime = DateTime.Today.AddHours(9).AddMinutes(30);
        var workHoursEndTime = DateTime.Today.AddHours(12).AddMinutes(30);
        
        var hearingStartTime = DateTime.Today.AddHours(11);
        var hearingEndTime = hearingStartTime.AddMinutes(480);
        
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new BookingsApi.Domain.DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime.TimeOfDay,
            EndTime = workHoursEndTime.TimeOfDay
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
        var workHoursStartTime = DateTime.Today.AddHours(9);
        var workHoursEndTime = DateTime.Today.AddHours(11);
        
        var hearingStartTime = DateTime.Today.AddHours(10);
        var hearingEndTime = hearingStartTime.AddMinutes(600);
        
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new BookingsApi.Domain.DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime.TimeOfDay,
            EndTime = workHoursEndTime.TimeOfDay
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