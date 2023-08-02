using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.V2;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Validation.V2;

public class EndpointRequestValidationTests
{
    private EndpointRequestValidation _validator;
        
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _validator = new EndpointRequestValidation();
    }
        
    [Test]
    public async Task Should_pass_validation()
    {
        var request = new EndpointRequest()
        {
            DisplayName = "EP1",
            DefenceAdvocateContactEmail = null
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }
        
    [Test]
    public async Task Should_return_missing_display_name_error()
    {
        var request = new EndpointRequest()
        {
            DisplayName = string.Empty,
            DefenceAdvocateContactEmail = null
        };
            
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.Any(x => x.ErrorMessage == EndpointRequestValidation.InvalidDisplayNameErrorMessage).Should().BeTrue();
    }
        
    [Test]
    public async Task Should_return_missing_defence_advocate_contact_email_error()
    {
        var request = new EndpointRequest()
        {
            DisplayName = "EP1",
            DefenceAdvocateContactEmail = string.Empty
        };
            
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.Any(x => x.ErrorMessage == EndpointRequestValidation.InvalidDefenceAdvocateContactEmailError).Should().BeTrue();
    }
        
    [Test]
    public async Task Should_return_invalid_defence_advocate_contact_email_error()
    {
        var request = new EndpointRequest()
        {
            DisplayName = "EP1",
            DefenceAdvocateContactEmail = "invalid"
        };
            
        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors.Any(x => x.ErrorMessage == EndpointRequestValidation.InvalidDefenceAdvocateContactEmailError).Should().BeTrue();
    }
}