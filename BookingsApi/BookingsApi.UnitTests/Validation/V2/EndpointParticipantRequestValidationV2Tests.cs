using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.V2;

namespace BookingsApi.UnitTests.Validation.V2;

public class EndpointParticipantRequestValidationV2Tests
{
    private EndpointParticipantsRequestValidationV2 _validator;
        
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _validator = new EndpointParticipantsRequestValidationV2();
    }
        
    [Test]
    public async Task Should_pass_validation()
    {
        var request = new EndpointParticipantsRequestV2()
        {
            ContactEmail = null
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }
        
    [Test]
    public async Task Should_return_missing_defence_advocate_contact_email_error()
    {
        var request = new EndpointParticipantsRequestV2()
        {
            ContactEmail= string.Empty
        };
            
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.Exists(x => x.ErrorMessage == EndpointParticipantsRequestValidationV2.InvalidContactEmailError).Should().BeTrue();
    }
        
    [Test]
    public async Task Should_return_invalid_defence_advocate_contact_email_error()
    {
        var request = new EndpointParticipantsRequestV2()
        {
            ContactEmail = "invalidemail"
        };
            
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.Exists(x => x.ErrorMessage == EndpointParticipantsRequestValidationV2.InvalidContactEmailError).Should().BeTrue();
    }
}