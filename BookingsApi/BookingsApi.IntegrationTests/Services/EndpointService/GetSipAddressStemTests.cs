using Autofac.Extras.Moq;
using BookingsApi.Common.Configuration;
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
    
    [TestCase(BookingSupplier.Vodafone)]
    [TestCase(null)]
    public void should_return_sip_address_stem_for_vodafone(BookingSupplier? supplier)
    {
        // arrange & act
        var result = _sut.GetSipAddressStem(supplier);
        
        // assert
        result.Should().Be("VodaConfigStem");
    }
}