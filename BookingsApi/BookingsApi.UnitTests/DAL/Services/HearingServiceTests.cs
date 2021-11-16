using Autofac.Extras.Moq;
using BookingsApi.DAL;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.DAL.Services
{
    public class HearingServiceTests
    {
        private AutoMock _mocker;
        private Mock<BookingsDbContext> _context;
        private HearingService _service;

        [SetUp]
        public void SetUp()
        {
            _mocker = AutoMock.GetLoose();

            var hearingRoles = new List<HearingRole>
            {
                new HearingRole(VideoHearingBuilder.JudgeHearingRoleId, HearingRoles.Judge),
                new HearingRole(VideoHearingBuilder.StaffMemberHearingRoleId, HearingRoles.StaffMember),
                new HearingRole(VideoHearingBuilder.LitigantInPersonHearingRole, HearingRoles.LitigantInPerson),
            };

            _context = _mocker.Mock<BookingsDbContext>();

            var hearingRolesDbSet = GetQueryableMockDbSet(hearingRoles);
            _context.Setup(x => x.HearingRoles).Returns(hearingRolesDbSet);

            _service = new HearingService(_context.Object);
        }

        [Test]
        public void Should_not_throw_domain_rule_exception_if_hearing_does_not_have_a_host()
        {
            //Arrange
            var hearing = new VideoHearingBuilder().Build();

            //Act/Assert
            Assert.DoesNotThrow(() => hearing.ValidateHostCount());
        }

        [Test]
        public void Should_throw_domain_rule_exception_if_hearing_does_not_have_a_host()
        {
            //Arrange
            var hearing = new VideoHearingBuilder().Build();
            hearing.Participants.Clear();
            hearing.Participants.Add(new ParticipantBuilder().IndividualParticipantApplicant);

            //Act/Assert
            Assert.Throws<DomainRuleException>(() => hearing.ValidateHostCount());
        }

        private DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet.Object;
        }
    }
}
