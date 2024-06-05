using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.V2;

namespace BookingsApi.UnitTests.Validation.V2;

public class EndpointRequestValidationV2Tests
{
    private EndpointRequestValidationV2 _validator;
        
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _validator = new EndpointRequestValidationV2();
    }
        
    [Test]
    public async Task Should_pass_validation()
    {
        var request = new EndpointRequestV2()
        {
            DisplayName = "EP1",
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }
        
    [Test]
    public async Task Should_return_missing_display_name_error()
    {
        var request = new EndpointRequestV2()
        {
            DisplayName = string.Empty,
        };
            
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Exists(x => x.ErrorMessage == EndpointRequestValidationV2.InvalidDisplayNameErrorMessage).Should().BeTrue();
    }
        
}