using BookingsApi.Domain;

namespace BookingsApi.UnitTests.Domain.EndPoints
{
    public class UpdateDisplayNameTests : DomainTests
    {
        private Endpoint _endpoint;

        [SetUp]
        public void Initialise()
        {
            _endpoint = new Endpoint(Guid.NewGuid().ToString(),"Original DisplayName", "sip@videohearings.net", "1234", null);
        }

        [Test]
        public async Task Should_update_display_name()
        {
            var newDisplayName = "New DisplayName";
            var originalUpdatedDate = _endpoint.UpdatedDate;

            await ApplyDelay();
            _endpoint.UpdateDisplayName(newDisplayName);
            
            _endpoint.DisplayName.Should().Be(newDisplayName);
            _endpoint.UpdatedDate.Should().BeAfter(originalUpdatedDate);
        }

        [Test]
        public async Task Should_throw_argument_null_exception_when_display_name_field_is_null()
        {
            var originalUpdatedDate = _endpoint.UpdatedDate;
            
            await ApplyDelay();
            Action action = () => _endpoint.UpdateDisplayName(string.Empty);
            
            action.Should().Throw<ArgumentNullException>();
            _endpoint.DisplayName.Should().Be("Original DisplayName");
            _endpoint.UpdatedDate.Should().Be(originalUpdatedDate);
        }

        [Test]
        public async Task Should_not_update_display_name_when_not_changed()
        {
            var originalDisplayName = _endpoint.DisplayName;
            var originalUpdatedDate = _endpoint.UpdatedDate;
            
            await ApplyDelay();
            _endpoint.UpdateDisplayName(_endpoint.DisplayName);
            
            _endpoint.DisplayName.Should().Be(originalDisplayName);
            _endpoint.UpdatedDate.Should().Be(originalUpdatedDate);
        }
    }
}
