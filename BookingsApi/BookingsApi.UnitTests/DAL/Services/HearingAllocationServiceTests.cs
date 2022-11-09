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
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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

        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
            _randomNumberGenerator = new Mock<IRandomNumberGenerator>();
            var configuration = GetDefaultSettings();
            _service = new HearingAllocationService(_context, _randomNumberGenerator.Object, configuration);
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

        #region ACs
        [Test]
        public async Task Should_allocate_successfully()
        {
            // Arrange
            var hearings = new List<VideoHearing>
            {
                CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(0), duration: 120),
                CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(30), duration: 150),
                CreateHearing(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(0), duration: 420),
                CreateHearing(DateTime.Today.AddDays(1).AddHours(11).AddMinutes(0), duration: 90)
            };
            
            var cso = SeedJusticeUser("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });   
            }

            foreach (var hearing in hearings)
            {
                // Act
                var result = await _service.AllocateCso(hearing.Id);
                
                // Assert
                result.Should().NotBeNull();
                result.Id.Should().Be(cso.Id);
            }
        }
        
        [Test]
        public async Task Should_fail_when_cso_has_no_work_hours()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0), duration: 60);

            var cso = SeedJusticeUser("user1@email.com", "User", "1");
            cso.VhoWorkHours.Clear();

            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await _service.AllocateCso(hearing.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, no CSOs available");
        }

        [Test]
        public async Task Should_fail_when_cso_is_already_allocated_to_maximum_number_of_concurrent_hearings()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.MaximumConcurrentHearings = 3;
            var service = new HearingAllocationService(_context, _randomNumberGenerator.Object, configuration);
            var hearing1 = CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(0), duration: 120);
            var hearing2 = CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(40), duration: 120);
            var hearing3 = CreateHearing(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(0), duration: 120);
            var hearing4 = CreateHearing(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(40), duration: 120);

            var cso = SeedJusticeUser("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });   
            }
            AllocateCsoToHearing(cso.Id, hearing1.Id);
            AllocateCsoToHearing(cso.Id, hearing2.Id);
            AllocateCsoToHearing(cso.Id, hearing3.Id);

            // Act
            var action = async() => await service.AllocateCso(hearing4.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing4.Id}, no CSOs available");
        }

        [TestCase("14:31")]
        [TestCase("15:29")]
        public async Task Should_fail_when_hearing_start_time_is_less_than_minimum_gap_of_existing_allocation(string hearingStartTime)
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.MinimumGapBetweenHearingsInMinutes = 30;
            var service = new HearingAllocationService(_context, _randomNumberGenerator.Object, configuration);
            var hearing1 = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0), duration: 60);
            var hearingStartTimeTimespan = TimeSpan.Parse(hearingStartTime);
            var hearing2 = CreateHearing(DateTime.Today.AddDays(1).AddHours(hearingStartTimeTimespan.Hours).AddMinutes(hearingStartTimeTimespan.Minutes));
            
            var cso = SeedJusticeUser("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });   
            }
            AllocateCsoToHearing(cso.Id, hearing1.Id);
            
            // Act
            var action = async() => await service.AllocateCso(hearing2.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing2.Id}, no CSOs available");
        }

        [Test]
        public async Task Should_allocate_successfully_to_cso_with_fewest_hearings_allocated()
        {
            // Arrange
            var hearing1 = CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(45));
            var hearing2 = CreateHearing(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(45));
            var hearing3 = CreateHearing(DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45));
            var hearing4 = CreateHearing(DateTime.Today.AddDays(1).AddHours(12).AddMinutes(45));
            var hearing5 = CreateHearing(DateTime.Today.AddDays(1).AddHours(13).AddMinutes(45));
            var hearing6 = CreateHearing(DateTime.Today.AddDays(1).AddHours(14).AddMinutes(45));
            var hearing7 = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(45));
            
            var cso1 = SeedJusticeUser("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso1.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateCsoToHearing(cso1.Id, hearing1.Id);
            AllocateCsoToHearing(cso1.Id, hearing4.Id);
            AllocateCsoToHearing(cso1.Id, hearing6.Id);
            
            var cso2 = SeedJusticeUser("user2@email.com", "User", "2");
            for (var i = 1; i <= 7; i++)
            {
                cso2.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateCsoToHearing(cso2.Id, hearing2.Id);
            AllocateCsoToHearing(cso2.Id, hearing5.Id);
            
            var cso3 = SeedJusticeUser("user3@email.com", "User", "3");
            for (var i = 1; i <= 7; i++)
            {
                cso3.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateCsoToHearing(cso3.Id, hearing3.Id);
            
            // Act
            var result = await _service.AllocateCso(hearing7.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso3.Id);
        }
        
        [TestCase(1)]
        [TestCase(2)]
        public async Task Should_allocate_randomly_when_multiple_csos_have_same_number_of_fewest_hearings(int generatedRandomNumber)
        {
            // Arrange
            var hearing1 = CreateHearing(DateTime.Today.AddDays(1).AddHours(9).AddMinutes(45));
            var hearing2 = CreateHearing(DateTime.Today.AddDays(1).AddHours(10).AddMinutes(45));
            var hearing3 = CreateHearing(DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45));
            var hearing4 = CreateHearing(DateTime.Today.AddDays(1).AddHours(12).AddMinutes(45));

            var cso1 = SeedJusticeUser("user1@email.com", "User", "1");
            for (var i = 1; i <= 7; i++)
            {
                cso1.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateCsoToHearing(cso1.Id, hearing1.Id);
            AllocateCsoToHearing(cso1.Id, hearing4.Id);

            var cso2 = SeedJusticeUser("user2@email.com", "User", "2");
            for (var i = 1; i <= 7; i++)
            {
                cso2.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateCsoToHearing(cso2.Id, hearing2.Id);

            var cso3 = SeedJusticeUser("user3@email.com", "User", "3");
            for (var i = 1; i <= 7; i++)
            {
                cso3.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            AllocateCsoToHearing(cso3.Id, hearing3.Id);
            
            var allocationCandidates = new List<Guid> { cso2.Id, cso3.Id };
            _randomNumberGenerator.Setup(x => x.Generate(It.IsAny<int>(), It.IsAny<int>())).Returns(generatedRandomNumber);
            
            // Act
            var result = await _service.AllocateCso(hearing4.Id);
            
            // Assert
            result.Should().NotBeNull();
            _randomNumberGenerator.Verify(c => c.Generate(1, allocationCandidates.Count), Times.AtLeastOnce);
            Assert.That(allocationCandidates.Contains(result.Id));
        }

        [Test]
        public async Task Should_fail_when_hearing_starts_before_cso_work_hours_start_time_and_setting_is_disabled()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToStartBeforeWorkStartTime = false;
            var service = new HearingAllocationService(_context, _randomNumberGenerator.Object, configuration);
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(7).AddMinutes(0), duration: 120);

            var cso = SeedJusticeUser("user1@email.com", "User", "1");
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
            var action = async() => await service.AllocateCso(hearing.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, no CSOs available");
        }
        
        [Test]
        public async Task Should_allocate_successfully_when_hearing_starts_after_cso_work_hours_start_time_and_setting_is_enabled()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToStartBeforeWorkStartTime = true;
            var service = new HearingAllocationService(_context, _randomNumberGenerator.Object, configuration);
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(7).AddMinutes(0), duration: 120);

            var cso = SeedJusticeUser("user1@email.com", "User", "1");
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
            var result = await service.AllocateCso(hearing.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso.Id);
        }
        
        [Test]
        public async Task Should_fail_when_hearing_ends_after_cso_work_hours_and_setting_is_disabled()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToEndAfterWorkEndTime = false;
            var service = new HearingAllocationService(_context, _randomNumberGenerator.Object, configuration);
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(16).AddMinutes(30), duration: 60);

            var cso = SeedJusticeUser("user1@email.com", "User", "1");
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
            var action = async() => await service.AllocateCso(hearing.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, no CSOs available");
        }
        
        [Test]
        public async Task Should_allocate_successfully_when_hearing_ends_after_cso_work_hours_and_setting_is_enabled()
        {
            // Arrange
            var configuration = GetDefaultSettings();
            configuration.AllowHearingToEndAfterWorkEndTime = true;
            var service = new HearingAllocationService(_context, _randomNumberGenerator.Object, configuration);
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(16).AddMinutes(30), duration: 60);

            var cso = SeedJusticeUser("user1@email.com", "User", "1");
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
            var result = await service.AllocateCso(hearing.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso.Id);
        }
        #endregion ACs
        
        #region Diagram and extra
        [Test]
        public void Should_fail_when_hearing_does_not_exist()
        {
            // Arrange
            var hearingId = Guid.NewGuid();

            // Act
            var action = async() => await _service.AllocateCso(hearingId);

            // Assert
            action.Should().Throw<ArgumentException>().And.Message.Should().Be($"Hearing {hearingId} not found");
        }
        
        [TestCase("08:00", "10:00")]
        [TestCase("12:00", "15:30")]
        [TestCase("15:30", "18:00")]
        [TestCase("18:00", "20:00")]
        public async Task Should_fail_when_no_csos_available_due_to_work_hours_not_coinciding(string workHourStartTime, string workHourEndTime)
        {
            // Arrange
            var hearingScheduledDateTime = DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0);
            var hearing = CreateHearing(hearingScheduledDateTime, duration: 60);

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
            var action = async() => await _service.AllocateCso(hearing.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, no CSOs available");
        }
        
        [TestCase("12:00", "15:30")]
        [TestCase("15:30", "18:00")]
        public async Task Should_fail_when_no_csos_available_due_to_non_availability_hours_coinciding(string nonAvailabilityStartTime, string nonAvailabilityEndTime)
        {
            // Arrange
            var hearingScheduledDateTime = DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0);
            var hearing = CreateHearing(hearingScheduledDateTime, duration: 60);

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
            
            // Act
            var action = async() => await _service.AllocateCso(hearing.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, no CSOs available");
        }

        [Test]
        public async Task Should_fail_when_work_hour_start_and_end_times_are_null()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0), 240);

            var cso = SeedJusticeUser("user1@email.com", "User", "1");
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
            var action = async() => await _service.AllocateCso(hearing.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, no CSOs available");
        }

        [Test]
        public async Task Should_fail_with_unsupported_error_when_hearing_spans_multiple_days()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(22).AddMinutes(0), 240);

            var cso = SeedJusticeUser("user1@email.com", "User", "1");
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
            var action = async() => await _service.AllocateCso(hearing.Id);
            
            // Assert
            action.Should().Throw<NotSupportedException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, hearings which span multiple days are not currently supported");
        }

        [Test]
        public async Task Should_allocate_successfully_when_one_cso_available_due_to_work_hours()
        {
            // Arrange
            var hearingScheduledDateTime = DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0);
            var hearing = CreateHearing(hearingScheduledDateTime);

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
            
            // Act
            var result = await _service.AllocateCso(hearing.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(availableCso.Id);
        }
        
        [Test]
        public async Task Should_allocate_successfully_when_one_cso_available_due_to_non_availabilities()
        {
            // Arrange
            var hearingScheduledDateTime = DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0);
            var hearing = CreateHearing(hearingScheduledDateTime);

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
            
            // Act
            var result = await _service.AllocateCso(hearing.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(availableCso.Id);
        }

        [Test]
        public async Task Should_ignore_non_availabilities_oustide_hearing_datetime()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0));

            var cso = SeedJusticeUser("user1@email.com", "User", "1");
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

            // Act
            var result = await _service.AllocateCso(hearing.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso.Id);
        }

        [Test]
        public async Task Should_target_cso_justice_users_only()
        {
            // Arrange
            var hearing = CreateHearing(DateTime.Today.AddDays(1).AddHours(15).AddMinutes(0));

            var cso = SeedJusticeUser("cso@email.com", "Cso", "1", (int)UserRoleId.Vho);
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

            int userRoleIndex = 1;
            foreach (int userRoleId in Enum.GetValues(typeof(UserRoleId)))
            {
                if (userRoleId == (int)UserRoleId.Vho)
                {
                    continue;
                }

                var nonCso = SeedJusticeUser($"nonCso{userRoleIndex}@email.com", "NonCso{i}", userRoleIndex.ToString(), userRoleId: userRoleId);
                for (var i = 1; i <= 7; i++)
                {
                    nonCso.VhoWorkHours.Add(new VhoWorkHours
                    {
                        DayOfWeekId = i,
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0)
                    });
                }

                userRoleIndex++;
            }

            // Act
            var action = async () => await _service.AllocateCso(hearing.Id);

            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, no CSOs available");
        }
        #endregion Diagram and extra

        private IList<JusticeUser> SeedJusticeUsers()
        {
            var user1 = SeedJusticeUser("user1@email.com", "User", "1");
            var user2 = SeedJusticeUser("user2@email.com", "User", "2");
            var user3 = SeedJusticeUser("user3@email.com", "User", "3");

            var justiceUsers = new List<JusticeUser> { user1, user2, user3 };
            return justiceUsers;
        }

        private JusticeUser SeedJusticeUser(string userName, string firstName, string lastName, int userRoleId = 2)
        {
            var justiceUser = new JusticeUser
            {
                ContactEmail = userName,
                Username = userName,
                UserRoleId = userRoleId,
                CreatedBy = "test@test.com",
                CreatedDate = DateTime.Now,
                FirstName = firstName,
                Lastname = lastName
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

        private void AllocateCsoToHearing(Guid justiceUserId, Guid hearingId)
        {
            // TODO can we move this to the Hearing domain model?
            _context.Allocations.Add(new Allocation
            {
                HearingId = hearingId,
                JusticeUserId = justiceUserId
            });
            _context.SaveChanges();
        }

        private AllocateCsoConfiguration GetDefaultSettings()
        {
            return new AllocateCsoConfiguration
            {
                AllowHearingToStartBeforeWorkStartTime = false,
                AllowHearingToEndAfterWorkEndTime = false,
                MinimumGapBetweenHearingsInMinutes = 30,
                MaximumConcurrentHearings = 3
            };
        }
    }
}
