using System;
using System.Collections.Generic;
using BookingsApi.Common.Services;
using BookingsApi.DAL;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Configuration;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Testing.Common.Builders.Domain;
using DayOfWeek = BookingsApi.Domain.DayOfWeek;

namespace BookingsApi.UnitTests.Services;

public class HearingAllocationServiceTests
{
    private HearingAllocationService _sut;
    

    [SetUp]
    public void Setup()
    {
        var settings = new AllocateHearingConfiguration();
        var options = Options.Create(settings);
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
        dbContextOptionsBuilder.UseInMemoryDatabase("VhBookings");
        var db = new BookingsDbContext(dbContextOptionsBuilder.Options);
        _sut = new HearingAllocationService(db, new RandomNumberGenerator(), options,
            new Logger<HearingAllocationService>(new LoggerFactory()));
    }

    [Test]
    public void should_set_work_hour_clashes_to_true_when_hearing_exceeds_cso_working_hours()
    {
        var workHoursStartTime = DateTime.Today.AddHours(9);
        var workHoursEndTime = DateTime.Today.AddHours(11);
        
        var hearingStartTime = DateTime.Today.AddHours(10);
        var hearing = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime).WithDuration(600).Build();
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime.TimeOfDay,
            EndTime = workHoursEndTime.TimeOfDay
        };
        var cso = Builder<JusticeUser>.CreateNew()
            .With(x => x.VhoWorkHours, new List<VhoWorkHours> {workHours})
            .Build();
        hearing.Allocations.Add(new Allocation {Hearing = hearing, JusticeUser = cso});

        var resultDtos = _sut.CheckForAllocationClashes(new List<VideoHearing> {hearing});
        resultDtos.Count.Should().Be(1);
        resultDtos[0].HasWorkHoursClash.Should().BeTrue();
    }

    [Test]
    public void should_set_work_hour_clashes_to_false_when_hearing_is_within_cso_working_hours()
    {
        var workHoursStartTime = DateTime.Today.AddHours(9);
        var workHoursEndTime = DateTime.Today.AddHours(18);

        var hearingStartTime = DateTime.Today.AddHours(10);
        var hearing = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime).WithDuration(60).Build();
        var workHours = new VhoWorkHours
        {
            DayOfWeek = new DayOfWeek {Day = hearingStartTime.DayOfWeek.ToString()},
            DayOfWeekId = (int) hearingStartTime.DayOfWeek,
            StartTime = workHoursStartTime.TimeOfDay,
            EndTime = workHoursEndTime.TimeOfDay
        };
        var cso = Builder<JusticeUser>.CreateNew()
            .With(x => x.VhoWorkHours, new List<VhoWorkHours> {workHours})
            .Build();
        hearing.Allocations.Add(new Allocation {Hearing = hearing, JusticeUser = cso});

        var resultDtos = _sut.CheckForAllocationClashes(new List<VideoHearing> {hearing});
        resultDtos.Count.Should().Be(1);
        resultDtos[0].HasWorkHoursClash.Should().BeFalse();
    }

    [Test] public void should_set_work_hour_clashes_to_null_when_hearing_does_not_have_an_allocated_cso()
    {
        var hearing = new VideoHearingBuilder().Build();
        
        var resultDtos = _sut.CheckForAllocationClashes(new List<VideoHearing>() {hearing});
        
        resultDtos.Count.Should().Be(1);
        resultDtos[0].HasWorkHoursClash.Should().BeNull();
    }
}