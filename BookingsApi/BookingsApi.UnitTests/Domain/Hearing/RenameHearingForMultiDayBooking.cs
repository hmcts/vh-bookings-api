using BookingsApi.Domain.Validations;
namespace BookingsApi.UnitTests.Domain.Hearing
{
    public class RenameHearingForMultiDayBooking
    {
        [Test]
        public void should_rename_hearing_for_multi_day_booking()
        {
            // Arrange
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.SourceId = Guid.NewGuid();
            var existingCase = hearing.GetCases().FirstOrDefault();
            var oldName = existingCase.Name;
            var newName = $"{existingCase.Name} Day 1 of 3";

            // Act
            hearing.RenameHearingForMultiDayBooking(newName);
            
            // Assert
            var updatedCase = hearing.GetCases().FirstOrDefault();
            updatedCase.Name.Should().NotBe(oldName);
            updatedCase.Name.Should().Be(newName);
        }

        [Test]
        public void should_throw_exception_when_hearing_is_not_multi_day()
        {
            // Arrange
            var hearing = new VideoHearingBuilder().WithCase().Build();
            hearing.SourceId = null; // Not multi-day
            var existingCase = hearing.GetCases().FirstOrDefault();
            var oldName = existingCase.Name;
            var newName = $"{existingCase.Name} Day 1 of 3";
            
            // Act
            var action = () => hearing.RenameHearingForMultiDayBooking(newName);
            
            // Assert
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Exists(x => x.Message == DomainRuleErrorMessages.HearingNotMultiDay).Should().BeTrue();
            
            var updatedCase = hearing.GetCases().FirstOrDefault();
            updatedCase.Name.Should().Be(oldName);
        }

        [Test]
        public void should_not_rename_hearing_when_hearing_has_no_cases()
        {
            // Arrange
            var hearing = new VideoHearingBuilder().Build();
            var newName = $"Test Day 1 of 3";

            // Act
            hearing.RenameHearingForMultiDayBooking(newName);
            
            // Assert
            Assert.DoesNotThrow(() => hearing.RenameHearingForMultiDayBooking(newName));
        }
    }
}
