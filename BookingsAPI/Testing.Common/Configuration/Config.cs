﻿using AcceptanceTests.Common.Api.Zap;
using Bookings.Common.Configuration;
using Bookings.DAL;
using Microsoft.EntityFrameworkCore;

namespace Testing.Common.Configuration
{
    public class Config
    {
        public AzureAdConfiguration AzureAdConfiguration { get; set; }
        public DbContextOptions<BookingsDbContext> BookingsDbContextOptions { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public ServicesConfiguration ServicesConfiguration { get; set; }
        public TestSettings TestSettings { get; set; }
        public ZapConfiguration ZapConfig { get; set; }
    }
}
