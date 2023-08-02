using BookingsApi.Common.Configuration;

namespace Testing.Common.Configuration
{
    public class Config
    {
        public AzureAdConfiguration AzureAdConfiguration { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public ServicesConfiguration ServicesConfiguration { get; set; }
        public TestSettings TestSettings { get; set; }
    }
}
