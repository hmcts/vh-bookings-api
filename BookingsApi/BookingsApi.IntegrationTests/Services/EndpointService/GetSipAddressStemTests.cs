using Autofac.Extras.Moq;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Services;
using BookingsApi.Contract.V2.Enums;
using Microsoft.Extensions.Options;

namespace BookingsApi.IntegrationTests.Services.EndpointService;

public class GetSipAddressStemTests
{
    private BookingsApi.Services.EndpointService _sut;
    private AutoMock _mocker;
    private SupplierConfiguration _supplierConfig;

    [SetUp]
    public void Setup()
    {
        _mocker = AutoMock.GetLoose();
        _supplierConfig = new SupplierConfiguration
        {
            SipAddressStemKinly = "KinlyConfigStem",
            SipAddressStemVodafone = "VodaConfigStem"
        };
        _mocker.Mock<IOptions<SupplierConfiguration>>().Setup(o => o.Value).Returns(_supplierConfig);
        _sut = _mocker.Create<BookingsApi.Services.EndpointService>();
    }
    
    [Test]
    public void should_return_sip_address_stem_for_kinly()
    {
        // arrange
        _mocker.Mock<IFeatureToggles>().Setup(x=> x.UseVodafoneToggle()).Returns(false);
        
        // act
        var result = _sut.GetSipAddressStem(null);
        
        // assert
        result.Should().Be("KinlyConfigStem");
    }
    
    [Test]
    public void should_return_sip_address_stem_for_vodafone()
    {
        // arrange
        _mocker.Mock<IFeatureToggles>().Setup(x=> x.UseVodafoneToggle()).Returns(true);
        
        // act
        var result = _sut.GetSipAddressStem(null);
        
        // assert
        result.Should().Be("VodaConfigStem");
    }
}