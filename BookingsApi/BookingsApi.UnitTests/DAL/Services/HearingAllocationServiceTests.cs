using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.DAL;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.UnitTests.Constants;
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
            _service = new HearingAllocationService(_context, _randomNumberGenerator.Object);
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
        public void Should_fail_when_hearing_does_not_exist()
        {
            // Arrange
            var hearingId = Guid.NewGuid();

            // Act
            var action = async() => await _service.AllocateCso(hearingId);

            // Assert
            action.Should().Throw<ArgumentException>().And.Message.Should().Be($"Hearing {hearingId} not found");
        }
        
        [Test]
        public async Task Should_fail_when_no_csos_available_due_to_work_hours_not_coinciding()
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
                        EndTime = new TimeSpan(10, 0, 0)
                    });
                }
            }

            await _context.SaveChangesAsync();
            
            // Act
            var action = async() => await _service.AllocateCso(hearing.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, no CSOs available");
        }
 
        [Test]
        public async Task Should_fail_when_no_csos_available_due_to_non_availability_hours_coinciding()
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
            
            // Act
            var action = async() => await _service.AllocateCso(hearing.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing.Id}, no CSOs available");
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
        public async Task Should_allocate_successfully_when_multiple_csos_available_and_one_with_no_allocations()
        {
            // Arrange
            var hearing1ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var hearing2ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(13).AddMinutes(45);
            var hearing3ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(15).AddMinutes(45);
            var hearing1 = CreateHearing(hearing1ScheduledDateTime);
            var hearing2 = CreateHearing(hearing2ScheduledDateTime);
            var hearing3 = CreateHearing(hearing3ScheduledDateTime);
            
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

            // Act
            var result = await _service.AllocateCso(hearing3.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso3.Id);
        }
        
        [Test]
        public async Task Should_allocate_successfully_when_multiple_csos_available_and_zero_with_no_allocations_and_one_with_one_allocation()
        {
            // Arrange
            var hearing1ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(9).AddMinutes(45);
            var hearing2ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(45);
            var hearing3ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var hearing4ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(12).AddMinutes(45);
            var hearing5ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(13).AddMinutes(45);
            var hearing6ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(14).AddMinutes(45);
            var hearing1 = CreateHearing(hearing1ScheduledDateTime);
            var hearing2 = CreateHearing(hearing2ScheduledDateTime);
            var hearing3 = CreateHearing(hearing3ScheduledDateTime);
            var hearing4 = CreateHearing(hearing4ScheduledDateTime);
            var hearing5 = CreateHearing(hearing5ScheduledDateTime);
            var hearing6 = CreateHearing(hearing6ScheduledDateTime);
            
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
            var result = await _service.AllocateCso(hearing6.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso3.Id);
        }
        
        // TODO confirm this behaviour with Murali
        [Test]
        public async Task Should_allocate_successfully_when_multiple_csos_available_and_zero_with_no_allocations_and_zero_with_one_allocation_and_one_with_two_allocations()
        {
            // Arrange
            var hearing1ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(8).AddMinutes(45);
            var hearing2ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(9).AddMinutes(45);
            var hearing3ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(45);
            var hearing4ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var hearing5ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(12).AddMinutes(45);
            var hearing6ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(13).AddMinutes(45);
            var hearing7ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(14).AddMinutes(45);
            var hearing8ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(15).AddMinutes(45);
            var hearing9ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(16).AddMinutes(45);
            var hearing1 = CreateHearing(hearing1ScheduledDateTime);
            var hearing2 = CreateHearing(hearing2ScheduledDateTime);
            var hearing3 = CreateHearing(hearing3ScheduledDateTime);
            var hearing4 = CreateHearing(hearing4ScheduledDateTime);
            var hearing5 = CreateHearing(hearing5ScheduledDateTime);
            var hearing6 = CreateHearing(hearing6ScheduledDateTime);
            var hearing7 = CreateHearing(hearing7ScheduledDateTime);
            var hearing8 = CreateHearing(hearing8ScheduledDateTime);
            var hearing9 = CreateHearing(hearing9ScheduledDateTime);
            
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
            AllocateCsoToHearing(cso1.Id, hearing7.Id);
            
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
            AllocateCsoToHearing(cso2.Id, hearing8.Id);
            
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
            AllocateCsoToHearing(cso3.Id, hearing6.Id);
            
            // Act
            var result = await _service.AllocateCso(hearing9.Id);
            
            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(cso3.Id);
        }
        
        [Test]
        public async Task Should_return_error_when_multiple_csos_available_and_zero_with_no_allocations_and_zero_with_one_allocation_and_zero_with_two_allocations()
        {
            // Arrange
            var hearing1ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(8).AddMinutes(30);
            var hearing2ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(9).AddMinutes(0);
            var hearing3ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(9).AddMinutes(30);
            var hearing4ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(0);
            var hearing5ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30);
            var hearing6ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(0);
            var hearing7ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(30);
            var hearing8ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(12).AddMinutes(0);
            var hearing9ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(12).AddMinutes(30);
            var hearing10ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(13).AddMinutes(0);
            var hearing11ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(13).AddMinutes(30);
            var hearing12ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(14).AddMinutes(0);
            var hearing1 = CreateHearing(hearing1ScheduledDateTime);
            var hearing2 = CreateHearing(hearing2ScheduledDateTime);
            var hearing3 = CreateHearing(hearing3ScheduledDateTime);
            var hearing4 = CreateHearing(hearing4ScheduledDateTime);
            var hearing5 = CreateHearing(hearing5ScheduledDateTime);
            var hearing6 = CreateHearing(hearing6ScheduledDateTime);
            var hearing7 = CreateHearing(hearing7ScheduledDateTime);
            var hearing8 = CreateHearing(hearing8ScheduledDateTime);
            var hearing9 = CreateHearing(hearing9ScheduledDateTime);
            var hearing10 = CreateHearing(hearing10ScheduledDateTime);
            var hearing11 = CreateHearing(hearing11ScheduledDateTime);
            var hearing12 = CreateHearing(hearing12ScheduledDateTime);
            
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
            AllocateCsoToHearing(cso1.Id, hearing7.Id);
            AllocateCsoToHearing(cso1.Id, hearing10.Id);
            
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
            AllocateCsoToHearing(cso2.Id, hearing8.Id);
            AllocateCsoToHearing(cso2.Id, hearing11.Id);
            
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
            AllocateCsoToHearing(cso3.Id, hearing6.Id);
            AllocateCsoToHearing(cso3.Id, hearing9.Id);
            
            // Act
            var action = async() => await _service.AllocateCso(hearing12.Id);
            
            // Assert
            action.Should().Throw<InvalidOperationException>().And.Message.Should().Be($"Unable to allocate to hearing {hearing12.Id}, no CSOs available");
        }
        
        [TestCase(1)]
        [TestCase(2)]
        public async Task Should_allocate_randomly_when_multiple_csos_available_and_more_than_one_with_no_allocations(int generatedRandomNumber)
        {
            // Arrange
            var hearing1ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var hearing2ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(13).AddMinutes(45);
            var hearing1 = CreateHearing(hearing1ScheduledDateTime);
            var hearing2 = CreateHearing(hearing2ScheduledDateTime);

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

            var allocationCandidates = new List<Guid> { cso2.Id, cso3.Id };
            _randomNumberGenerator.Setup(x => x.Generate(It.IsAny<int>(), It.IsAny<int>())).Returns(generatedRandomNumber);
            
            // Act
            var result = await _service.AllocateCso(hearing2.Id);
            
            // Assert
            result.Should().NotBeNull();
            _randomNumberGenerator.Verify(c => c.Generate(1, allocationCandidates.Count), Times.AtLeastOnce);
            Assert.That(allocationCandidates.Contains(result.Id));
        }
        
        [TestCase(1)]
        [TestCase(2)]
        public async Task Should_allocate_randomly_when_multiple_csos_available_and_zero_with_no_allocations_and_more_than_one_with_one_allocation(int generatedRandomNumber)
        {
            // Arrange
            var hearing1ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(9).AddMinutes(45);
            var hearing2ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(45);
            var hearing3ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var hearing4ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(12).AddMinutes(45);
            var hearing5ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(13).AddMinutes(45);
            var hearing1 = CreateHearing(hearing1ScheduledDateTime);
            var hearing2 = CreateHearing(hearing2ScheduledDateTime);
            var hearing3 = CreateHearing(hearing3ScheduledDateTime);
            var hearing4 = CreateHearing(hearing4ScheduledDateTime);
            var hearing5 = CreateHearing(hearing5ScheduledDateTime);

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
            var result = await _service.AllocateCso(hearing5.Id);
            
            // Assert
            result.Should().NotBeNull();
            _randomNumberGenerator.Verify(c => c.Generate(1, allocationCandidates.Count), Times.AtLeastOnce);
            Assert.That(allocationCandidates.Contains(result.Id));
        }
        
        [TestCase(1)]
        [TestCase(2)]
        public async Task Should_allocate_randomly_when_multiple_csos_available_and_zero_with_no_allocations_and_zero_with_one_allocation_and_more_than_one_with_two_allocations(int generatedRandomNumber)
        {
            // Arrange
            var hearing1ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(8).AddMinutes(45);
            var hearing2ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(9).AddMinutes(45);
            var hearing3ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(45);
            var hearing4ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var hearing5ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(12).AddMinutes(45);
            var hearing6ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(13).AddMinutes(45);
            var hearing7ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(14).AddMinutes(45);
            var hearing8ScheduledDateTime = DateTime.Today.AddDays(1).AddHours(15).AddMinutes(45);
            var hearing1 = CreateHearing(hearing1ScheduledDateTime);
            var hearing2 = CreateHearing(hearing2ScheduledDateTime);
            var hearing3 = CreateHearing(hearing3ScheduledDateTime);
            var hearing4 = CreateHearing(hearing4ScheduledDateTime);
            var hearing5 = CreateHearing(hearing5ScheduledDateTime);
            var hearing6 = CreateHearing(hearing6ScheduledDateTime);
            var hearing7 = CreateHearing(hearing7ScheduledDateTime);
            var hearing8 = CreateHearing(hearing8ScheduledDateTime);

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
            AllocateCsoToHearing(cso1.Id, hearing7.Id);
            
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
            AllocateCsoToHearing(cso3.Id, hearing6.Id);
            
            var allocationCandidates = new List<Guid> { cso2.Id, cso3.Id };
            _randomNumberGenerator.Setup(x => x.Generate(It.IsAny<int>(), It.IsAny<int>())).Returns(generatedRandomNumber);
            
            // Act
            var result = await _service.AllocateCso(hearing8.Id);
            
            // Assert
            result.Should().NotBeNull();
            _randomNumberGenerator.Verify(c => c.Generate(1, allocationCandidates.Count), Times.AtLeastOnce);
            Assert.That(allocationCandidates.Contains(result.Id));
        }
        
        /*
         * Questions and testing improvements
         *
         * Should we avoid allocating csos to hearings that overlap with one they are already assigned to?
         * Need to test that it doesn't try to assign to a non-CSO
         * Need to test that the query which gets a cso's allocations only gets those in the same time frame as the hearing
         */

        private IList<JusticeUser> SeedJusticeUsers()
        {
            var user1 = SeedJusticeUser("user1@email.com", "User", "1");
            var user2 = SeedJusticeUser("user2@email.com", "User", "2");
            var user3 = SeedJusticeUser("user3@email.com", "User", "3");

            var justiceUsers = new List<JusticeUser> { user1, user2, user3 };
            return justiceUsers;
        }

        private JusticeUser SeedJusticeUser(string userName, string firstName, string lastName, bool isTeamLead = false)
        {
            var justiceUser = new JusticeUser
            {
                ContactEmail = userName,
                Username = userName,
                UserRoleId = isTeamLead ? (int)UserRoleId.vhTeamLead : (int)UserRoleId.vho,
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

        private VideoHearing CreateHearing(DateTime scheduledDateTime)
        {
            var duration = 80;
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
    }
}
