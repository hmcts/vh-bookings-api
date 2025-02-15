﻿using BookingsApi.Domain.Validations;

namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class ValidateHostCountTests
    {
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
            var hearing = new VideoHearingBuilder(addJudge:false).Build();
            
            //Act/Assert
            Assert.Throws<DomainRuleException>(() => hearing.ValidateHostCount());
        }
    }
}
