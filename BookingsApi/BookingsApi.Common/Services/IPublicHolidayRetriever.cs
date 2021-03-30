using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BookingsApi.Common.Models;
using Newtonsoft.Json;

namespace BookingsApi.Common.Services
{
    public interface IPublicHolidayRetriever
    {
        Task<List<PublicHoliday>> RetrieveUpcomingHolidays();
    }

    public class UkPublicHolidayRetriever : IPublicHolidayRetriever
    {
        private readonly HttpClient _httpClient;

        public UkPublicHolidayRetriever(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PublicHoliday>> RetrieveUpcomingHolidays()
        {
            var holidays = new List<PublicHoliday>();
            var ukHolidaysUri = @"https://www.gov.uk/bank-holidays.json";
            var response = await _httpClient.GetAsync(ukHolidaysUri);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var ukHolidays = JsonConvert.DeserializeObject<UkHolidaysResponse>(json);
                var englandAndWales = ukHolidays.EnglandAndWales.Events;
                holidays = englandAndWales.Where(x => x.Date >= DateTime.Today).ToList();
            }

            return holidays;
        }
    }
}