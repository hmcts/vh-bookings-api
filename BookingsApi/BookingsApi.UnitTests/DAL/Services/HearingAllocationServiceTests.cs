using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Services;
using BookingsApi.DAL;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.DAL.Services
{
    public class HearingAllocationServiceTests
    {
        private BookingsDbContext _context;
        private HearingAllocationService _service;
        private CaseType _caseType;
        private HearingType _hearingType;
        private HearingVenue _hearingVenue;
        private Mock<IRandomNumberGenerator> _randomNumberGenerator;
        private UserRole _userRoleCso;
        private UserRole _userRoleVhTeamLead;

        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
            _randomNumberGenerator = new Mock<IRandomNumberGenerator>();
            var configuration = GetDefaultSettings();
            _service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration));
            SeedRefData();
        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [TearDown]
        public async Task TearDown()
        {
            _context.VideoHearings.RemoveRange(_context.Set<VideoHearing>().ToList());
            _context.JusticeUsers.RemoveRange(_context.Set<JusticeUser>().ToList());
            await _context.SaveChangesAsync();
        }

        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully()
        {
            // Arrange
            var hearings = new List<VideoHearing>
            {
                CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(0), duration: 120),
                CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(30), duration: 150),
                CreateHearing(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(0), duration: 420),
                CreateHearing(DateTime.Today.AddDays(1).AddHours(11).AddMinutes(0), duration: 90)
            };
            
            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });   
            }

            await _context.SaveChangesAsync();

            foreach (var hearing in hearings)
            {
                // Act
                var result = await _service.AllocateAutomatically(hearing.Id);
                
                // Assert
                result.Should().NotBeNull();
                result.Id.Should().Be(cso.Id);
            }
        }
        
        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Cso_Has_No_Work_Hours()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0), duration: 60);

            var cso = SeedCso("user1@email.com", "User", "1");
            cso.VhoWorkHours.Clear();

            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await _service.AllocateAutomatically(hearing.Id);
            
            // Assert
            AssertNoCsosAvailableError(action, hearing.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Cso_Is_Already_Allocated_To_Maximum_Concurrent_Hearings()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.MaximumConcurrentHearings = 3;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration));
            var hearing1 = CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(0), duration: 120);
            var hearing2 = CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(40), duration: 120);
            var hearing3 = CreateHearing(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(0), duration: 120);
            var hearing4 = CreateHearing(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(40), duration: 120);

            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });   
            }
            AllocateAutomaticallyToHearing(cso.Id, hearing1.Id);
            AllocateAutomaticallyToHearing(cso.Id, hearing2.Id);
            AllocateAutomaticallyToHearing(cso.Id, hearing3.Id);

            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await service.AllocateAutomatically(hearing4.Id);
            
            // Assert
            AssertNoCsosAvailableError(action, hearing4.Id);
        }

        [TestCase("14:31")]
        [TestCase("15:29")]
        public async Task AllocateAutomatically_Should_Fail_When_Time_Gap_Between_Hearings_Is_Less_Than_Minimum(string hearingStartTime)
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.MinimumGapBetweenHearingsInMinutes = 30;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration));
            var hearing1 = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0), duration: 60);
            var hearingStartTimeTimespan = TimeSpan.Parse(hearingStartTime);
            var hearing2 = CreateHearing(DateTime.Today.AddDays(1).AddHours(hearingStartTimeTimespan.Hours).AddMinutes(hearingStartTimeTimespan.Minutes));
            
            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });   
            }
            AllocateAutomaticallyToHearing(cso.Id, hearing1.Id);
            
            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await service.AllocateAutomatically(hearing2.Id);
            
            // Assert
            AssertNoCsosAvailableError(action, hearing2.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully_To_Cso_With_Fewest_Hearings_Allocated()
        {
            // Arrange
            var hearing1 = CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(45));
            var hearing2 = CreateHearing(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(45));
            var hearing3 = CreateHearing(DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45));
            var hearing4 = CreateHearing(DateTime.Today.AddDays(1).AddHours(12).AddMinutes(45));
            var hearing5 = CreateHearing(DateTime.Today.AddDays(1).AddHours(13).AddMinutes(45));
            var hearing6 = CreateHearing(DateTime.Today.AddDays(1).AddHours(14).AddMinutes(45));
            var hearing7 = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(45));
            
            var cso1 = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso1.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateAutomaticallyToHearing(cso1.Id, hearing1.Id);
            AllocateAutomaticallyToHearing(cso1.Id, hearing4.Id);
            AllocateAutomaticallyToHearing(cso1.Id, hearing6.Id);
            
            var cso2 = SeedCso("user2@email.com", "User", "2");
            for (var i = 1; i <= 7; i++)
            {
                cso2.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateAutomaticallyToHearing(cso2.Id, hearing2.Id);
            AllocateAutomaticallyToHearing(cso2.Id, hearing5.Id);
            
            var cso3 = SeedCso("user3@email.com", "User", "3");
            for (var i = 1; i <= 7; i++)
            {
                cso3.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateAutomaticallyToHearing(cso3.Id, hearing3.Id);
            
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateAutomatically(hearing7.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso3.Id);
        }
        
        [TestCase(1)]
        [TestCase(2)]
        public async Task AllocateAutomatically_Should_Allocate_Randomly_When_Multiple_Csos_Have_Same_Number_Of_Fewest_Hearings(int generatedRandomNumber)
        {
            // Arrange
            var hearing1 = CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(45));
            var hearing2 = CreateHearing(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(45));
            var hearing3 = CreateHearing(DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45));
            var hearing4 = CreateHearing(DateTime.Today.AddDays(1).AddHours(12).AddMinutes(45));
            var hearing5 = CreateHearing(DateTime.Today.AddDays(1).AddHours(13).AddMinutes(45));

            var cso1 = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso1.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateAutomaticallyToHearing(cso1.Id, hearing1.Id);
            AllocateAutomaticallyToHearing(cso1.Id, hearing4.Id);

            var cso2 = SeedCso("user2@email.com", "User", "2");
            for (var i = 1; i <= 7; i++)
            {
                cso2.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateAutomaticallyToHearing(cso2.Id, hearing2.Id);

            var cso3 = SeedCso("user3@email.com", "User", "3");
            for (var i = 1; i <= 7; i++)
            {
                cso3.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateAutomaticallyToHearing(cso3.Id, hearing3.Id);
            
            await _context.SaveChangesAsync();
            
            var allocationCandidates = new List<Guid> { cso2.Id, cso3.Id };
            _randomNumberGenerator.Setup(x => x.Generate(It.IsAny<int>(), It.IsAny<int>())).Returns(generatedRandomNumber);
            
            // Act
            var result = await _service.AllocateAutomatically(hearing5.Id);
            
            // Assert
            result.Should().NotBeNull();
            _randomNumberGenerator.Verify(c => c.Generate(1, allocationCandidates.Count), Times.AtLeastOnce);
            Assert.That(allocationCandidates.Contains(result.Id));
        }

        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Hearing_Starts_Before_Cso_Work_Hours_Start_Time_And_Setting_Is_Disabled()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToStartBeforeWorkStartTime = false;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration));
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(7).AddMinutes(0), duration: 120);

            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }

            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await service.AllocateAutomatically(hearing.Id);
            
            // Assert
            AssertNoCsosAvailableError(action, hearing.Id);
        }
        
        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully_When_Hearing_Starts_After_Cso_Work_Hours_Start_Time_And_Setting_Is_Enabled()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToStartBeforeWorkStartTime = true;
            var service = new HearingAllocationService(
                _context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration));
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(7).AddMinutes(0), duration: 120);

            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }

            await _context.SaveChangesAsync();
            
            // Act
            var result = await service.AllocateAutomatically(hearing.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso.Id);
        }
        
        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Hearing_Ends_After_Cso_Work_Hours_And_Setting_Is_Disabled()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToEndAfterWorkEndTime = false;
            var service = new HearingAllocationService(
                _context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration));
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(16).AddMinutes(30), duration: 60);

            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }

            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await service.AllocateAutomatically(hearing.Id);
            
            // Assert
            AssertNoCsosAvailableError(action, hearing.Id);
        }
        
        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully_When_Hearing_Ends_After_Cso_Work_Hours_And_Setting_Is_Enabled()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToEndAfterWorkEndTime = true;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration));
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(16).AddMinutes(30), duration: 60);

            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }

            await _context.SaveChangesAsync();
            
            // Act
            var result = await service.AllocateAutomatically(hearing.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso.Id);
        }

        [Test]
        public void AllocateAutomatically_Should_Fail_When_Hearing_Does_Not_Exist()
        {
            // Arrange
            var hearingId = Guid.NewGuid();

            // Act
            var action = async() => await _service.AllocateAutomatically(hearingId);

            // Assert
            action.Should().Throw<DomainRuleException>().And.Message.Should().Be($"Hearing {hearingId} not found");
        }
        
        [TestCase("08:00", "10:00")]
        [TestCase("12:00", "15:30")]
        [TestCase("15:30", "18:00")]
        [TestCase("18:00", "20:00")]
        public async Task AllocateAutomatically_Should_Fail_When_No_Csos_Available_Due_To_Work_Hours_Not_Coinciding(string workHourStartTime, string workHourEndTime)
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0), duration: 60);

            var justiceUsers = SeedJusticeUsers();
            foreach (var justiceUser in justiceUsers)
            {
                for (var i = 1; i <= 7; i++)
                {
                    justiceUser.VhoWorkHours.Add(new VhoWorkHours
                    {
                        DayOfWeekId = i, 
                        StartTime = TimeSpan.Parse(workHourStartTime), 
                        EndTime = TimeSpan.Parse(workHourEndTime)
                    });
                }
            }

            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await _service.AllocateAutomatically(hearing.Id);
            
            // Assert
            AssertNoCsosAvailableError(action, hearing.Id);
        }
        
        [TestCase("12:00", "15:30")]
        [TestCase("15:30", "18:00")]
        public async Task AllocateAutomatically_Should_Fail_When_No_Csos_Available_Due_To_Non_Availability_Hours_Coinciding(string nonAvailabilityStartTime, string nonAvailabilityEndTime)
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0), duration: 60);

            var justiceUsers = SeedJusticeUsers();
            foreach (var justiceUser in justiceUsers)
            {
                for (var i = 1; i <= 7; i++)
                {
                    justiceUser.VhoWorkHours.Add(new VhoWorkHours
                    {
                        DayOfWeekId = i, 
                        StartTime = new TimeSpan(8, 0, 0), 
                        EndTime = new TimeSpan(17, 0, 0)
                    });
                }
                
                justiceUser.VhoNonAvailability.Add(new VhoNonAvailability
                {
                    StartTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day)
                        .Add(TimeSpan.Parse(nonAvailabilityStartTime)),
                    EndTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day)
                        .Add(TimeSpan.Parse(nonAvailabilityEndTime))
                });
            }
            
            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await _service.AllocateAutomatically(hearing.Id);
            
            // Assert
            AssertNoCsosAvailableError(action, hearing.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Work_Hour_Start_And_End_Times_Are_Null()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0), 240);

            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = null, 
                    EndTime = null
                });
            }
        
            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await _service.AllocateAutomatically(hearing.Id);
            
            // Assert
            AssertNoCsosAvailableError(action, hearing.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Hearing_Spans_Multiple_Days()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(22).AddMinutes(0), 240);

            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
        
            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await _service.AllocateAutomatically(hearing.Id);
            
            // Assert
            action.Should().Throw<DomainRuleException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, hearings which span multiple days are not currently supported");
        }

        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully_When_One_Cso_Available_Due_To_Work_Hours()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0));

            var justiceUsers = SeedJusticeUsers();
            var availableCso = justiceUsers.First();
            for (var i = 1; i <= 7; i++)
            {
                availableCso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateAutomatically(hearing.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(availableCso.Id);
        }
        
        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully_When_One_Cso_Available_Due_To_Non_Availabilities()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0));

            var justiceUsers = SeedJusticeUsers();
            foreach (var justiceUser in justiceUsers)
            {
                for (var i = 1; i <= 7; i++)
                {
                    justiceUser.VhoWorkHours.Add(new VhoWorkHours
                    {
                        DayOfWeekId = i, 
                        StartTime = new TimeSpan(8, 0, 0), 
                        EndTime = new TimeSpan(17, 0, 0)
                    });
                }
                
                justiceUser.VhoNonAvailability.Add(new VhoNonAvailability
                {
                    StartTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 13, 0 ,0),
                    EndTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 17, 0 ,0)
                });
            }

            var availableCso = justiceUsers.First();
            availableCso.VhoNonAvailability.Clear();
            
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateAutomatically(hearing.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(availableCso.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Ignore_Non_Availabilities_Oustide_Hearing_Datetime()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0));

            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            cso.VhoNonAvailability.Add(new VhoNonAvailability
            {
                StartTime = new DateTime(hearing.ScheduledDateTime.Year + 1, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 12, 0, 0),
                EndTime = new DateTime(hearing.ScheduledDateTime.Year + 1, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 18, 0, 0)
            });
            cso.VhoNonAvailability.Add(new VhoNonAvailability
            {
                StartTime = new DateTime(hearing.ScheduledDateTime.Year-1, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 12, 0, 0),
                EndTime = new DateTime(hearing.ScheduledDateTime.Year-1, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 18, 0, 0)
            });

            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateAutomatically(hearing.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Target_Cso_Justice_Users_Only()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0));

            var cso = SeedCso("cso@email.com", "Cso", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            cso.VhoNonAvailability.Add(new VhoNonAvailability
            {
                StartTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 12, 0, 0),
                EndTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 18, 0, 0)
            });

            var nonCso = SeedNonCso($"nonCso@email.com", "NonCso", "1");
            for (var i = 1; i <= 7; i++)
            {
                nonCso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }

            await _context.SaveChangesAsync();
            
            // Act
            var action = async () => await _service.AllocateAutomatically(hearing.Id);

            // Assert
            AssertNoCsosAvailableError(action, hearing.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Hearing_Already_Allocated()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(45));

            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateAutomaticallyToHearing(cso.Id, hearing.Id);

            await _context.SaveChangesAsync();
            
            // Assert
            var action = async () => await _service.AllocateAutomatically(hearing.Id);

            // Assert
            action.Should().Throw<DomainRuleException>().And.Message.Should().Be($"Hearing {hearing.Id} has already been allocated");
        }

        [Test]
        public async Task AllocateAutomatically_Should_Ignore_Deleted_Non_Availabilities()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0));

            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            cso.VhoNonAvailability.Add(new VhoNonAvailability
            {
                StartTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 12, 0, 0),
                EndTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 18, 0, 0),
                Deleted = true
            });

            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateAutomatically(hearing.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso.Id);
        }

        private IList<JusticeUser> SeedJusticeUsers()
        {
            var user1 = SeedCso("user1@email.com", "User", "1");
            var user2 = SeedCso("user2@email.com", "User", "2");
            var user3 = SeedCso("user3@email.com", "User", "3");

            var justiceUsers = new List<JusticeUser> { user1, user2, user3 };
            return justiceUsers;
        }

        private JusticeUser SeedCso(string userName, string firstName, string lastName)
        {
            return SeedJusticeUser(userName, firstName, lastName, _userRoleCso);
        }

        private JusticeUser SeedNonCso(string userName, string firstName, string lastName)
        {
            return SeedJusticeUser(userName, firstName, lastName, _userRoleVhTeamLead);
        }

        private JusticeUser SeedJusticeUser(string userName, string firstName, string lastName, UserRole userRole)
        {
            var justiceUser = new JusticeUser
            {
                ContactEmail = userName,
                Username = userName,
                UserRoleId = userRole.Id,
                CreatedBy = "test@test.com",
                CreatedDate = DateTime.Now,
                FirstName = firstName,
                Lastname = lastName,
                UserRole = userRole
            };

            var workHours = new List<VhoWorkHours>();
            justiceUser.SetProtected(nameof(justiceUser.VhoWorkHours), workHours);

            var nonAvailabilities = new List<VhoNonAvailability>();
            justiceUser.SetProtected(nameof(justiceUser.VhoNonAvailability), nonAvailabilities);

            _context.JusticeUsers.Add(justiceUser);
            _context.SaveChanges();

            return justiceUser;
        }

        private void SeedRefData()
        {
            var caseTypeName = "Generic";
            var hearingTypeName = "Automated Test";
            var hearingVenueName = "Birmingham Civil and Family Justice Centre";

            var caseType = new CaseType(1, caseTypeName);
            _context.CaseTypes.Add(caseType);
            _caseType = caseType;
            
            _hearingType = Builder<HearingType>.CreateNew().WithFactory(() => new HearingType(hearingTypeName)).Build();

            var refDataBuilder = new RefDataBuilder();
            _hearingVenue = refDataBuilder.HearingVenues.First( x=> x.Name == hearingVenueName);

            _userRoleCso = new UserRole((int)UserRoleId.Vho, "Video hearings officer");
            _userRoleVhTeamLead = new UserRole((int)UserRoleId.VhTeamLead, "Video hearings team lead");
            
            _context.SaveChanges();
        }

        private VideoHearing CreateHearing(DateTime scheduledDateTime, int duration = 60)
        {
            var hearingRoomName = "Room03";
            var otherInformation = "OtherInformation03";
            var createdBy = "User03";
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;
            var cancelReason = "Online abandonment (incomplete registration)";

            var videoHearing = Builder<VideoHearing>.CreateNew().WithFactory(() =>
                    new VideoHearing(_caseType, _hearingType, scheduledDateTime, duration, _hearingVenue, hearingRoomName,
                        otherInformation, createdBy, questionnaireNotRequired, audioRecordingRequired, cancelReason))
                .Build();

            // Set the navigation properties as well since these would've been set if we got the hearing from DB
            videoHearing.SetProtected(nameof(videoHearing.HearingType), _hearingType);
            videoHearing.SetProtected(nameof(videoHearing.CaseType), _caseType);
            videoHearing.SetProtected(nameof(videoHearing.HearingVenue), _hearingVenue);

            _context.VideoHearings.Add(videoHearing);
            _context.SaveChanges();

            return videoHearing;
        }

        private void AllocateAutomaticallyToHearing(Guid justiceUserId, Guid hearingId)
        {
            _context.Allocations.Add(new Allocation
            {
                HearingId = hearingId,
                JusticeUserId = justiceUserId
            });
            _context.SaveChanges();
        }

        private static AllocateHearingConfiguration GetDefaultSettings()
        {
            return new AllocateHearingConfiguration
            {
                AllowHearingToStartBeforeWorkStartTime = false,
                AllowHearingToEndAfterWorkEndTime = false,
                MinimumGapBetweenHearingsInMinutes = 30,
                MaximumConcurrentHearings = 3
            };
        }

        private static void AssertNoCsosAvailableError(Func<Task<JusticeUser>> action, Guid hearingId)
        {
            action.Should().Throw<DomainRuleException>().And.Message.Should().Be($"Unable to allocate to hearing {hearingId}, no CSOs available");
        }
    }
}
