using BookingsApi.Domain;

namespace BookingsApi.UnitTests.Domain.EndPoints
{
    public class UpdateDisplayNameTests
    {
        private Endpoint _endpoint;

        [SetUp]
        public void Initialise()
        {
            _endpoint = new Endpoint(Guid.NewGuid().ToString(),"Original DisplayName", "sip@videohearings.net", "1234", null);
        }

        [Test]
        public void Should_update_display_name()
        {
            const string newDisplayName = "New DisplayName";
            _endpoint.SetProtected(nameof(_endpoint.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
            var originalUpdatedDate = _endpoint.UpdatedDate;
            
            _endpoint.UpdateDisplayName(newDisplayName);
            
            _endpoint.DisplayName.Should().Be(newDisplayName);
            _endpoint.UpdatedDate.Should().BeAfter(originalUpdatedDate);
        }

        [Test]
        public void Should_throw_argument_null_exception_when_display_name_field_is_null()
        {
            _endpoint.SetProtected(nameof(_endpoint.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
            var originalUpdatedDate = _endpoint.UpdatedDate;
            
            var action = () => _endpoint.UpdateDisplayName(string.Empty);
            
            action.Should().Throw<ArgumentNullException>();
            _endpoint.DisplayName.Should().Be("Original DisplayName");
            _endpoint.UpdatedDate.Should().Be(originalUpdatedDate);
        }

        [Test]
        public void Should_not_update_display_name_when_not_changed()
        {
            var originalDisplayName = _endpoint.DisplayName;
            _endpoint.SetProtected(nameof(_endpoint.UpdatedDate), DateTime.UtcNow.AddMinutes(-1));
            var originalUpdatedDate = _endpoint.UpdatedDate;
            
            _endpoint.UpdateDisplayName(_endpoint.DisplayName);
            
            _endpoint.DisplayName.Should().Be(originalDisplayName);
            _endpoint.UpdatedDate.Should().Be(originalUpdatedDate);
        }
    }
}
