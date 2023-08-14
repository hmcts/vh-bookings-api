using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.DAL;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Configuration;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JusticeUser = BookingsApi.Domain.JusticeUser;

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
        private Mock<ILogger<HearingAllocationService>> _logger;

        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
            _randomNumberGenerator = new Mock<IRandomNumberGenerator>();
            var configuration = GetDefaultSettings();
            _logger = new Mock<ILogger<HearingAllocationService>>();
            
            _service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);
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
                CreateHearing(new DateTime(DateTime.Today.Year + 1, 7, 18, 8, 0, 0, DateTimeKind.Utc), duration: 120),
                CreateHearing(new DateTime(DateTime.Today.Year + 2, 3, 4, 9, 0, 0, DateTimeKind.Utc), duration: 120),
                CreateHearing(new DateTime(DateTime.Today.Year + 2, 3, 4, 9, 30, 0, DateTimeKind.Utc), duration: 150),
                CreateHearing(new DateTime(DateTime.Today.Year + 2, 3, 4, 10, 0, 0, DateTimeKind.Utc), duration: 420),
                CreateHearing(new DateTime(DateTime.Today.Year + 2, 3, 4, 11, 0, 0, DateTimeKind.Utc), duration: 90),
                CreateHearing(new DateTime(DateTime.Today.Year + 2, 3, 4, 15, 0, 0, DateTimeKind.Utc), duration: 120)
            };
            
            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(9, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });   
            }

            await _context.SaveChangesAsync();

            foreach (var hearing in hearings)
            {
                // Act
                var result = await _service.AllocateAutomatically(hearing.Id);
                
                // Assert
                AssertCsoAllocated(result, cso, hearing);
            }
        }
        
        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Cso_Has_No_Work_Hours()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc), duration: 60);

            var cso = SeedCso("user1@email.com", "User", "1");
            cso.VhoWorkHours.Clear();

            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await _service.AllocateAutomatically(hearing.Id);
            
            // Assert
            await AssertNoCsosAvailableError(action, hearing.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Cso_Is_Already_Allocated_To_Maximum_Concurrent_Hearings()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.MaximumConcurrentHearings = 3;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);
            var hearing1 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 9, 0, 0, DateTimeKind.Utc), duration: 120);
            var hearing2 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 9, 40, 0, DateTimeKind.Utc), duration: 120);
            var hearing3 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 10, 0, 0, DateTimeKind.Utc), duration: 120);
            var hearing4 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 10, 40, 0, DateTimeKind.Utc), duration: 120);

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
            AllocateAutomaticallyToHearing(cso, hearing1);
            AllocateAutomaticallyToHearing(cso, hearing2);
            AllocateAutomaticallyToHearing(cso, hearing3);

            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await service.AllocateAutomatically(hearing4.Id);
            
            // Assert
            await AssertNoCsosAvailableError(action, hearing4.Id);
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
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);
            var hearing1 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc), duration: 60);
            var hearingStartTimeTimespan = TimeSpan.Parse(hearingStartTime);
            var hearing2 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, hearingStartTimeTimespan.Hours, hearingStartTimeTimespan.Minutes, 0, DateTimeKind.Utc));
            
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
            AllocateAutomaticallyToHearing(cso, hearing1);
            
            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await service.AllocateAutomatically(hearing2.Id);
            
            // Assert
            await AssertNoCsosAvailableError(action, hearing2.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully_To_Cso_With_Fewest_Hearings_Allocated()
        {
            // Arrange
            var hearing1 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 9, 45, 0, DateTimeKind.Utc));
            var hearing2 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 10, 45, 0, DateTimeKind.Utc));
            var hearing3 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 11, 45, 0, DateTimeKind.Utc));
            var hearing4 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 12, 45, 0, DateTimeKind.Utc));
            var hearing5 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 13, 45, 0, DateTimeKind.Utc));
            var hearing6 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 14, 45, 0, DateTimeKind.Utc));
            var hearing7 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 45, 0, DateTimeKind.Utc));
            
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
            AllocateAutomaticallyToHearing(cso1, hearing1);
            AllocateAutomaticallyToHearing(cso1, hearing4);
            AllocateAutomaticallyToHearing(cso1, hearing6);
            
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
            AllocateAutomaticallyToHearing(cso2, hearing2);
            AllocateAutomaticallyToHearing(cso2, hearing5);
            
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
            AllocateAutomaticallyToHearing(cso3, hearing3);
            
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateAutomatically(hearing7.Id);
            
            // Assert
            AssertCsoAllocated(result, cso3, hearing7);
        }
        
        [TestCase(1)]
        [TestCase(2)]
        public async Task AllocateAutomatically_Should_Allocate_Randomly_When_Multiple_Csos_Have_Same_Number_Of_Fewest_Hearings(int generatedRandomNumber)
        {
            // Arrange
            var hearing1 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 9, 45, 0, DateTimeKind.Utc));
            var hearing2 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 10, 0, 0, DateTimeKind.Utc));
            var hearing3 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 11, 45, 0, DateTimeKind.Utc));
            var hearing4 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 12, 45, 0, DateTimeKind.Utc));
            var hearing5 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 13, 45, 0, DateTimeKind.Utc));

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
            AllocateAutomaticallyToHearing(cso1, hearing1);
            AllocateAutomaticallyToHearing(cso1, hearing4);

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
            AllocateAutomaticallyToHearing(cso2, hearing2);

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
            AllocateAutomaticallyToHearing(cso3, hearing3);
            
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
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 7, 0, 0, DateTimeKind.Utc), duration: 120);

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
            await AssertNoCsosAvailableError(action, hearing.Id);
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
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 7, 0, 0, DateTimeKind.Utc), duration: 120);

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
            AssertCsoAllocated(result, cso, hearing);
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
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 16, 30, 0, DateTimeKind.Utc), duration: 60);

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
            await AssertNoCsosAvailableError(action, hearing.Id);
        }
        
        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully_When_Hearing_Ends_After_Cso_Work_Hours_And_Setting_Is_Enabled()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToEndAfterWorkEndTime = true;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 16, 30, 0, DateTimeKind.Utc), duration: 60);

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
            AssertCsoAllocated(result, cso, hearing);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Hearing_Does_Not_Exist()
        {
            // Arrange
            var hearingId = Guid.NewGuid();

            // Act
            var action = async() => await _service.AllocateAutomatically(hearingId);

            // Assert
            await action.Should().ThrowAsync<DomainRuleException>().WithMessage($"Hearing {hearingId} not found");
        }
        
        [TestCase("08:00", "10:00")]
        [TestCase("12:00", "15:30")]
        [TestCase("15:30", "18:00")]
        [TestCase("18:00", "20:00")]
        public async Task AllocateAutomatically_Should_Fail_When_No_Csos_Available_Due_To_Work_Hours_Not_Coinciding(string workHourStartTime, string workHourEndTime)
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc), duration: 60);

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
           await AssertNoCsosAvailableError(action, hearing.Id);
        }
        
        [TestCase("12:00", "15:30")]
        [TestCase("15:30", "18:00")]
        public async Task AllocateAutomatically_Should_Fail_When_No_Csos_Available_Due_To_Non_Availability_Hours_Coinciding(string nonAvailabilityStartTime, string nonAvailabilityEndTime)
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc), duration: 60);

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
                    StartTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 0, 0, 0, DateTimeKind.Utc)
                        .Add(TimeSpan.Parse(nonAvailabilityStartTime)),
                    EndTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 0, 0, 0, DateTimeKind.Utc)
                        .Add(TimeSpan.Parse(nonAvailabilityEndTime))
                });
            }
            
            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await _service.AllocateAutomatically(hearing.Id);
            
            // Assert
            await AssertNoCsosAvailableError(action, hearing.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Work_Hour_Start_And_End_Times_Are_Null()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc), 240);

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
            await AssertNoCsosAvailableError(action, hearing.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Hearing_Spans_Multiple_Days()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 22, 0, 0, DateTimeKind.Utc), 240);

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
            await action.Should().ThrowAsync<DomainRuleException>().WithMessage($"Unable to allocate to hearing {hearing.Id}, hearings which span multiple days are not currently supported");
        }

        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully_When_One_Cso_Available_Due_To_Work_Hours()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));

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
            AssertCsoAllocated(result, availableCso, hearing);
        }
        
        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully_When_One_Cso_Available_Due_To_Non_Availabilities()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));

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
                    StartTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 13, 0 ,0, DateTimeKind.Utc),
                    EndTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 17, 0 ,0, DateTimeKind.Utc)
                });
            }

            var availableCso = justiceUsers.First();
            availableCso.VhoNonAvailability.Clear();
            
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateAutomatically(hearing.Id);
            
            // Assert
            AssertCsoAllocated(result, availableCso, hearing);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Ignore_Non_Availabilities_Oustide_Hearing_Datetime()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));

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
                StartTime = new DateTime(hearing.ScheduledDateTime.Year + 1, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 12, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(hearing.ScheduledDateTime.Year + 1, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 18, 0, 0, DateTimeKind.Utc)
            });
            cso.VhoNonAvailability.Add(new VhoNonAvailability
            {
                StartTime = new DateTime(hearing.ScheduledDateTime.Year-1, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 12, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(hearing.ScheduledDateTime.Year-1, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 18, 0, 0, DateTimeKind.Utc)
            });

            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateAutomatically(hearing.Id);

            // Assert
            AssertCsoAllocated(result, cso, hearing);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Target_Cso_Justice_Users_Only()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));

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
                StartTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 12, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 18, 0, 0, DateTimeKind.Utc)
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
            await AssertNoCsosAvailableError(action, hearing.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Fail_When_Hearing_Already_Allocated()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 9, 45, 0, DateTimeKind.Utc));

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
            AllocateAutomaticallyToHearing(cso, hearing);

            await _context.SaveChangesAsync();
            
            // Assert
            var action = async () => await _service.AllocateAutomatically(hearing.Id);

            // Assert
            await action.Should().ThrowAsync<DomainRuleException>().WithMessage($"Hearing {hearing.Id} has already been allocated");
        }

        [Test]
        public async Task AllocateAutomatically_Should_Ignore_Deleted_Non_Availabilities()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));

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
                StartTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 12, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 18, 0, 0, DateTimeKind.Utc),
                Deleted = true
            });

            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateAutomatically(hearing.Id);

            // Assert
            AssertCsoAllocated(result, cso, hearing);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Ignore_Deleted_Csos()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));

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
                StartTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 12, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 18, 0, 0, DateTimeKind.Utc),
                Deleted = true
            });
            cso = await _context.JusticeUsers.FirstOrDefaultAsync(x => x.Id == cso.Id);
            cso.Delete();

            await _context.SaveChangesAsync();
            
            // Act
            var action = async () => await _service.AllocateAutomatically(hearing.Id);

            // Assert
            await AssertNoCsosAvailableError(action, hearing.Id);
        }

        [Test]
        public async Task AllocateAutomatically_Should_Allocate_Successfully_With_Future_Allocations_Present()
        {
            // If a user has been allocated to a hearing for a future date, they should still be a candidate for allocation before this date
            
            // Arrange
            var cso = SeedCso("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            
            var futureHearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 7, 24, 9, 0, 0, DateTimeKind.Utc));
            AllocateAutomaticallyToHearing(cso, futureHearing);
            
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 7, 21, 9, 0, 0, DateTimeKind.Utc));
            
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateAutomatically(hearing.Id);

            // Assert
            AssertCsoAllocated(result, cso, hearing);
        }

        [Test]
        public async Task DeallocateFromUnavailableHearings_Should_Deallocate_When_User_Not_Available_Due_To_Work_Hours()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));
            
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
            await _service.AllocateAutomatically(hearing.Id);
            var foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().NotBeNull();
            foundHearing.AllocatedTo.Id.Should().Be(cso.Id);
            foreach (var workHour in cso.VhoWorkHours)
            {
                workHour.StartTime = new TimeSpan(20, 0, 0);
                workHour.EndTime = new TimeSpan(22, 0, 0);
            }
            await _context.SaveChangesAsync();

            // Act
            await _service.DeallocateFromUnavailableHearings(cso.Id);

            // Assert
            foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().BeNull();
            AssertMessageLogged($"Deallocated hearing {hearing.Id}", LogLevel.Information);
        }
        
        [Test]
        public async Task DeallocateFromUnavailableHearings_Should_Deallocate_When_User_Not_Available_Due_To_Non_Availabilties()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));
            
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
            await _service.AllocateAutomatically(hearing.Id);
            var foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().NotBeNull();
            foundHearing.AllocatedTo.Id.Should().Be(cso.Id);
            cso.VhoNonAvailability.Add(new VhoNonAvailability
            {
                StartTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 12, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(hearing.ScheduledDateTime.Year, hearing.ScheduledDateTime.Month, hearing.ScheduledDateTime.Day, 18, 0, 0, DateTimeKind.Utc)
            });
            await _context.SaveChangesAsync();

            // Act
            await _service.DeallocateFromUnavailableHearings(cso.Id);

            // Assert
            foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().BeNull();
            AssertMessageLogged($"Deallocated hearing {hearing.Id}", LogLevel.Information);
        }

        [Test]
        public async Task DeallocateFromUnavailableHearings_Should_Deallocate_When_Hearing_Starts_Before_Work_Hours_Start_Time_And_Setting_Is_Disabled()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));
            
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
            await _service.AllocateAutomatically(hearing.Id);
            var foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().NotBeNull();
            foundHearing.AllocatedTo.Id.Should().Be(cso.Id);
            foreach (var workHour in cso.VhoWorkHours)
            {
                workHour.StartTime = new TimeSpan(20, 0, 0);
                workHour.EndTime = new TimeSpan(22, 0, 0);
            }
            await _context.SaveChangesAsync();
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToStartBeforeWorkStartTime = false;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);

            // Act
            await service.DeallocateFromUnavailableHearings(cso.Id);

            // Assert
            foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().BeNull();
        }

        [Test]
        public async Task DeallocateFromUnavailableHearings_Should_Deallocate_When_Hearing_Ends_After_Work_Hours_Start_Time_And_Setting_Is_Disabled()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));
            
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
            await _service.AllocateAutomatically(hearing.Id);
            var foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().NotBeNull();
            foundHearing.AllocatedTo.Id.Should().Be(cso.Id);
            foreach (var workHour in cso.VhoWorkHours)
            {
                workHour.StartTime = new TimeSpan(10, 0, 0);
                workHour.EndTime = new TimeSpan(12, 0, 0);
            }
            await _context.SaveChangesAsync();
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToEndAfterWorkEndTime = false;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);

            // Act
            await service.DeallocateFromUnavailableHearings(cso.Id);

            // Assert
            foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().BeNull();
        }
        
        [Test]
        public async Task DeallocateFromUnavailableHearings_Should_Not_Deallocate_When_Hearing_Starts_Before_Work_Hours_Start_Time_And_Setting_Is_Enabled()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));
            
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
            await _service.AllocateAutomatically(hearing.Id);
            var foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().NotBeNull();
            foundHearing.AllocatedTo.Id.Should().Be(cso.Id);
            foreach (var workHour in cso.VhoWorkHours)
            {
                workHour.StartTime = new TimeSpan(15, 30, 0);
                workHour.EndTime = new TimeSpan(20, 0, 0);
            }
            await _context.SaveChangesAsync();
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToStartBeforeWorkStartTime = true;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);

            // Act
            await service.DeallocateFromUnavailableHearings(cso.Id);

            // Assert
            foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().NotBeNull();
            foundHearing.AllocatedTo.Id.Should().Be(cso.Id);
        }

        [Test]
        public async Task DeallocateFromUnavailableHearings_Should_Not_Deallocate_When_Hearing_Ends_After_Work_Hours_Start_Time_And_Setting_Is_Enabled()
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));
            
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
            await _service.AllocateAutomatically(hearing.Id);
            var foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().NotBeNull();
            foundHearing.AllocatedTo.Id.Should().Be(cso.Id);
            foreach (var workHour in cso.VhoWorkHours)
            {
                workHour.StartTime = new TimeSpan(12, 0, 0);
                workHour.EndTime = new TimeSpan(15, 30, 0);
            }
            await _context.SaveChangesAsync();
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToEndAfterWorkEndTime = true;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);

            // Act
            await service.DeallocateFromUnavailableHearings(cso.Id);

            // Assert
            foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().NotBeNull();
            foundHearing.AllocatedTo.Id.Should().Be(cso.Id);
        }

        [TestCase("08:00", "10:00", false, false)]
        [TestCase("20:00", "22:00", false, false)]
        [TestCase("08:00", "10:00", true, false)]
        [TestCase("20:00", "22:00", true, false)]
        [TestCase("08:00", "10:00", false, true)]
        [TestCase("20:00", "22:00", false, true)]
        [TestCase("08:00", "10:00", true, true)]
        [TestCase("20:00", "22:00", true, true)]
        public async Task DeallocateFromUnavailableHearings_Should_Deallocate_When_Work_Hours_Are_Outside_Hearing_Datetime(string workHoursStartTime, 
            string workHoursEndTime,
            bool allowHearingToStartBeforeWorkStartTime,
            bool allowHearingToEndAfterWorkEndTime)
        {
            // Arrange
            var hearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));
            
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
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToStartBeforeWorkStartTime = allowHearingToStartBeforeWorkStartTime;
            configuration.AllowHearingToEndAfterWorkEndTime = allowHearingToEndAfterWorkEndTime;
            var service = new HearingAllocationService(_context, 
                _randomNumberGenerator.Object, 
                new OptionsWrapper<AllocateHearingConfiguration>(configuration),
                _logger.Object);
            await service.AllocateAutomatically(hearing.Id);
            var foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().NotBeNull();
            foundHearing.AllocatedTo.Id.Should().Be(cso.Id);
            foreach (var workHour in cso.VhoWorkHours)
            {
                workHour.StartTime = TimeSpan.Parse(workHoursStartTime);
                workHour.EndTime = TimeSpan.Parse(workHoursEndTime);
            }
            await _context.SaveChangesAsync();

            // Act
            await service.DeallocateFromUnavailableHearings(cso.Id);

            // Assert
            foundHearing = await _context.VideoHearings.FindAsync(hearing.Id);
            foundHearing.AllocatedTo.Should().BeNull();
        }

        [Test]
        public async Task DeallocateFromUnavailableHearings_Should_Ignore_Historical_Allocations()
        {
            // Arrange
            var historicalHearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));
            var historicalStartDate = new DateTime(2019, 3, 1, 15, 0, 0, DateTimeKind.Utc);
            historicalHearing.SetProtected(nameof(historicalHearing.ScheduledDateTime), historicalStartDate); // Necessary to bypass the validation for start date being in the past
            var futureHearing = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 15, 0, 0, DateTimeKind.Utc));
            
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
            await _service.AllocateAutomatically(historicalHearing.Id);
            await _service.AllocateAutomatically(futureHearing.Id);
            var foundHistoricalHearing = await _context.VideoHearings.FindAsync(historicalHearing.Id);
            foundHistoricalHearing.AllocatedTo.Should().NotBeNull();
            foundHistoricalHearing.AllocatedTo.Id.Should().Be(cso.Id);
            var foundFutureHearing = await _context.VideoHearings.FindAsync(futureHearing.Id);
            foundFutureHearing.AllocatedTo.Should().NotBeNull();
            foundFutureHearing.AllocatedTo.Id.Should().Be(cso.Id);
            foreach (var workHour in cso.VhoWorkHours)
            {
                workHour.StartTime = new TimeSpan(20, 0, 0);
                workHour.EndTime = new TimeSpan(22, 0, 0);
            }
            await _context.SaveChangesAsync();
            
            // Act
            await _service.DeallocateFromUnavailableHearings(cso.Id);
            
            // Assert
            foundHistoricalHearing = await _context.VideoHearings.FindAsync(historicalHearing.Id);
            foundHistoricalHearing.AllocatedTo.Should().NotBeNull();
            foundHistoricalHearing.AllocatedTo.Id.Should().Be(cso.Id);
            foundFutureHearing = await _context.VideoHearings.FindAsync(futureHearing.Id);
            foundFutureHearing.AllocatedTo.Should().BeNull();
        }

        [Test]
        public async Task DeallocateFromUnavailableHearings_Should_Throw_Exception_When_User_Not_Found()
        {
            // Arrange
            var justiceUserId = Guid.NewGuid();

            // Assert
            var action = async () => await _service.DeallocateFromUnavailableHearings(justiceUserId);

            // Assert
            await action.Should().ThrowAsync<DomainRuleException>()
                .WithMessage($"Justice user {justiceUserId} not found");
        }
        
        [Test]
        public async Task AllocateManually_Should_Allocate_Successfully_To_Cso_Overriding_Allocated_Cso()
        {
            // Arrange
            var hearing1 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 9, 45, 0, DateTimeKind.Utc));
            var hearing2 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 11, 45, 0, DateTimeKind.Utc));
            var hearing3 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 13, 45, 0, DateTimeKind.Utc));

            var list = new List<Guid>();
            list.Add(hearing1.Id);
            list.Add(hearing2.Id);
            list.Add(hearing3.Id);

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
            AllocateAutomaticallyToHearing(cso1, hearing1);
            AllocateAutomaticallyToHearing(cso1, hearing3);
            
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
            
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateHearingsToCso(list, cso3.Id);
            
            // Assert
            AssertCsoAllocatedListHearing(result, cso3, list);
        }
        
        [Test]
        public async Task AllocateManually_Should_Allocate_Successfully_To_Cso_For_Not_Allocated_Hearing()
        {
            // Arrange
            var hearing1 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 9, 45, 0, DateTimeKind.Utc));
            var hearing2 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 2, 3, 45, 0, DateTimeKind.Utc));

            var list = new List<Guid>();
            list.Add(hearing1.Id);
            list.Add(hearing2.Id);
            
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
            AllocateAutomaticallyToHearing(cso1, hearing1);
            
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
            
            await _context.SaveChangesAsync();
            
            // Act
            var result = await _service.AllocateHearingsToCso(list, cso3.Id);
            
            // Assert
            AssertCsoAllocatedListHearing(result, cso3, list);
        }
        
        [Test]
        public async Task AllocateManually_Should_Throw_Domain_Exception_Not_Found_Cso()
        {
            // Arrange
            var hearing1 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 1, 9, 45, 0, DateTimeKind.Utc));
            var hearing2 = CreateHearing(new DateTime(DateTime.Today.Year + 1, 3, 2, 3, 45, 0, DateTimeKind.Utc));

            var list = new List<Guid>();
            list.Add(hearing1.Id);
            list.Add(hearing2.Id);
            
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
            AllocateAutomaticallyToHearing(cso1, hearing1);

            await _context.SaveChangesAsync();

            var noCsoGuid = Guid.NewGuid();
            // Act
            var result = async () => await _service.AllocateHearingsToCso(list, noCsoGuid);
            
            // Assert
            var ex = await result.Should().ThrowAsync<DomainRuleException>();
            ex.And.ValidationFailures.Should().Contain(v =>
                v.Message == $"Unable to allocate to hearing {hearing1.Id}, with CSO {noCsoGuid}");
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
            return SeedJusticeUser(userName, firstName, lastName, new []{_userRoleCso});
        }

        private JusticeUser SeedNonCso(string userName, string firstName, string lastName)
        {
            return SeedJusticeUser(userName, firstName, lastName, new []{_userRoleVhTeamLead});
        }

        private JusticeUser SeedJusticeUser(string userName, string firstName, string lastName, IEnumerable<UserRole> userRoles)
        {
            var justiceUser = new JusticeUser()
            {
                ContactEmail = userName,
                Username = userName,
                CreatedBy = "test@test.com",
                CreatedDate = DateTime.Now,
                FirstName = firstName,
                Lastname = lastName,
                JusticeUserRoles = new List<JusticeUserRole>()
            };
            
            foreach (var role in userRoles)
                justiceUser.JusticeUserRoles.Add(new JusticeUserRole(justiceUser, role));
            

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

        private void AllocateAutomaticallyToHearing(JusticeUser justiceUser, VideoHearing hearing)
        {
            hearing.AllocateVho(justiceUser);
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

        private static async Task AssertNoCsosAvailableError(Func<Task<JusticeUser>> action, Guid hearingId)
        {
            await action.Should().ThrowAsync<DomainRuleException>()
                .WithMessage($"Unable to allocate to hearing {hearingId}, no CSOs available");
        }

        private void AssertCsoAllocated(JusticeUser actualCso, JusticeUser expectedCso, Hearing hearing)
        {
            actualCso.Should().NotBeNull();
            actualCso.Id.Should().Be(expectedCso.Id);
            var allocation = hearing.Allocations.SingleOrDefault(a => a.HearingId == hearing.Id && a.JusticeUserId == expectedCso.Id);
            allocation.Should().NotBeNull();
        }
        
        private static void AssertCsoAllocatedListHearing(List<VideoHearing> list, JusticeUser expectedCso, List<Guid> hearingIds)
        {
            expectedCso.Should().NotBeNull();
            list.Count.Should().Be(hearingIds.Count);
        }
        
        private void AssertMessageLogged(string expectedMessage, LogLevel expectedLogLevel)
        {
            _logger.Verify(x => x.Log(
                It.Is<LogLevel>(log => log == expectedLogLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == expectedMessage && @type.Name == "FormattedLogValues"),
                It.Is<Exception>(x => x == null),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }
}
