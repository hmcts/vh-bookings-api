using System;
using System.Collections.Generic;
using BookingsApi.Common.Services;
using BookingsApi.DAL;
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

namespace BookingsApi.IntegrationTests.Services.HearingAllocationService;

public class CheckForAllocationClashesTests
{
    private DAL.Services.HearingAllocationService _sut;
    

    [SetUp]
    public void Setup()
    {
        var settings = new AllocateHearingConfiguration();
        var options = Options.Create(settings);
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
        dbContextOptionsBuilder.UseInMemoryDatabase("VhBookings");
        var db = new BookingsDbContext(dbContextOptionsBuilder.Options);
        _sut = new DAL.Services.HearingAllocationService(db, new RandomNumberGenerator(), options,
            new Logger<DAL.Services.HearingAllocationService>(new LoggerFactory()));
    }
    
    [Test]
    public void should_set_concurrency_to_4_when_four_hearings_overlap()
    {
        // arrange
        var hearingStartTime = DateTime.Today.AddHours(10);
        
        var hearing1 = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime).WithDuration(600).Build();
        var hearing2 = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime.AddHours(1)).WithDuration(600).Build();
        var hearing3 = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime.AddHours(2)).WithDuration(600).Build();
        var hearing4 = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime.AddHours(3)).WithDuration(600).Build();
        
        var cso = Builder<JusticeUser>.CreateNew()
            .Build();
        hearing1.Allocations.Add(new Allocation {Hearing = hearing1, JusticeUser = cso});
        hearing2.Allocations.Add(new Allocation {Hearing = hearing2, JusticeUser = cso});
        hearing3.Allocations.Add(new Allocation {Hearing = hearing3, JusticeUser = cso});
        hearing4.Allocations.Add(new Allocation {Hearing = hearing4, JusticeUser = cso});

        // act
        var resultDtos = _sut.CheckForAllocationClashes(new List<VideoHearing> {hearing1, hearing2, hearing3, hearing4});
        
        // assert
        resultDtos.Count.Should().Be(4);
        resultDtos[0].ConcurrentHearingsCount.Should().Be(4);
    }
    
    [Test]
    public void should_set_concurrency_to_0_when_no_hearings_overlap()
    {
        // arrange
        var hearingStartTime = DateTime.Today.AddHours(10);
        
        var hearing1 = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime).WithDuration(10).Build();
        var hearing2 = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime.AddHours(2)).WithDuration(10).Build();
        var hearing3 = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime.AddHours(4)).WithDuration(10).Build();
        var hearing4 = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime.AddHours(6)).WithDuration(10).Build();
        
        var cso = Builder<JusticeUser>.CreateNew()
            // .With(x => x.VhoWorkHours, new List<VhoWorkHours> {workHours})
            .Build();
        hearing1.Allocations.Add(new Allocation {Hearing = hearing1, JusticeUser = cso});
        hearing2.Allocations.Add(new Allocation {Hearing = hearing2, JusticeUser = cso});
        hearing3.Allocations.Add(new Allocation {Hearing = hearing3, JusticeUser = cso});
        hearing4.Allocations.Add(new Allocation {Hearing = hearing4, JusticeUser = cso});

        // act
        var resultDtos = _sut.CheckForAllocationClashes(new List<VideoHearing> {hearing1, hearing2, hearing3, hearing4});
        
        // assert
        resultDtos.Count.Should().Be(4);
        resultDtos[0].ConcurrentHearingsCount.Should().Be(0);
    }

    
    [Test]
    public void should_set_work_hour_clashes_to_true_when_hearing_exceeds_cso_working_hours()
    {
        // arrange
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

        // act
        var resultDtos = _sut.CheckForAllocationClashes(new List<VideoHearing> {hearing});
        
        // assert
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

    [Test] public void should_set_clashes_to_null_when_hearing_does_not_have_an_allocated_cso()
    {
        var hearing = new VideoHearingBuilder().Build();
        
        var resultDtos = _sut.CheckForAllocationClashes(new List<VideoHearing>() {hearing});
        
        resultDtos.Count.Should().Be(1);
        resultDtos[0].HasWorkHoursClash.Should().BeNull();
        resultDtos[0].HasNonAvailabilityClash.Should().BeNull();
        resultDtos[0].ConcurrentHearingsCount.Should().BeNull();
    }

    [Test]
    public void should_set_nonavailability_clash_to_true_when_hearing_is_on_cso_non_available_time()
    {
        // arrange
        var nonAvailableStartTime = DateTime.Today.AddHours(9);
        var nonAvailableEndTime = DateTime.Today.AddHours(11);
        
        var hearingStartTime = DateTime.Today.AddHours(10);
        var hearing = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime).WithDuration(600).Build();
        var vhoNonAvailabilities = new List<VhoNonAvailability>()
        {
            new(1) {StartTime = nonAvailableStartTime, EndTime = nonAvailableEndTime}
        };
        var cso = Builder<JusticeUser>.CreateNew()
            .With(x=> x.VhoNonAvailability, vhoNonAvailabilities)
            // .With(x => x.VhoWorkHours, new List<VhoWorkHours> {workHours})
            .Build();
        hearing.Allocations.Add(new Allocation {Hearing = hearing, JusticeUser = cso});

        // act
        var resultDtos = _sut.CheckForAllocationClashes(new List<VideoHearing> {hearing});
        
        // assert
        resultDtos.Count.Should().Be(1);
        resultDtos[0].HasNonAvailabilityClash.Should().BeTrue();
    }
    
    [Test]
    public void should_set_nonavailability_clash_to_false_when_hearing_is_not_on_cso_non_available_time()
    {
        // arrange
        var nonAvailableStartTime = DateTime.Today.AddHours(8);
        var nonAvailableEndTime = DateTime.Today.AddHours(9).AddMinutes(59);
        
        var hearingStartTime = DateTime.Today.AddHours(10);
        var hearing = new VideoHearingBuilder().WithScheduledDateTime(hearingStartTime).WithDuration(600).Build();
        var vhoNonAvailabilities = new List<VhoNonAvailability>()
        {
            new(1) {StartTime = nonAvailableStartTime, EndTime = nonAvailableEndTime}
        };
        var cso = Builder<JusticeUser>.CreateNew()
            .With(x=> x.VhoNonAvailability, vhoNonAvailabilities)
            // .With(x => x.VhoWorkHours, new List<VhoWorkHours> {workHours})
            .Build();
        hearing.Allocations.Add(new Allocation {Hearing = hearing, JusticeUser = cso});

        // act
        var resultDtos = _sut.CheckForAllocationClashes(new List<VideoHearing> {hearing});
        
        // assert
        resultDtos.Count.Should().Be(1);
        resultDtos[0].HasNonAvailabilityClash.Should().BeFalse();
    }
}