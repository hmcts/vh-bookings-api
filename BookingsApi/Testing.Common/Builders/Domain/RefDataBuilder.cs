using System;
using System.Collections.Generic;
using BookingsApi.Domain;

namespace Testing.Common.Builders.Domain
{
    public class RefDataBuilder
    {
        public List<HearingVenue> HearingVenues { get; set; }
        public List<JusticeUser> Users { get; set; }


        public RefDataBuilder()
        {
            InitHearingVenues();
            InitUsers();
        }
        
        private void InitHearingVenues()
        {
            HearingVenues = new List<HearingVenue>()
            {
                new HearingVenue(1, "Birmingham Civil and Family Justice Centre", false, true, "1"),
                new HearingVenue(2, "Manchester Civil and Family Justice Centre", false, true, "2"),
                new HearingVenue(3, "Taylor House Tribunal Hearing Centre", false, true, "3"),
            };
        }
        
        private void InitUsers()
        {
            Users = new List<JusticeUser>()
            {
                new JusticeUser
                {
                    ContactEmail = "email@email.com",
                    Username = "email@email.com",
                    CreatedBy = "integration.test@test.com",
                    CreatedDate = DateTime.Now,
                    FirstName = "firstName",
                    Lastname = "lastName",
                },
                new JusticeUser
                {
                    ContactEmail = "email1@email.com",
                    Username = "email1@email.com",
                    CreatedBy = "integration.test@test.com",
                    CreatedDate = DateTime.Now,
                    FirstName = "firstName1",
                    Lastname = "lastName1",
                },
                new JusticeUser
                {
                    ContactEmail = "email2@email.com",
                    Username = "email2@email.com",
                    CreatedBy = "integration.test@test.com",
                    CreatedDate = DateTime.Now,
                    FirstName = "firstName2",
                    Lastname = "lastName2",
                }
            };
        }
    }
}