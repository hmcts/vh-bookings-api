﻿using System;
using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain
{
    public class JudiciaryPersonStaging : AggregateRoot<Guid>
    {
        private readonly DateTime _currentUTC = DateTime.UtcNow;
        
        public JudiciaryPersonStaging(string externalRefId, string personalCode, string title, string knownAs, string surname,
            string fullname, string postNominals, string email, string hasLeft, string leaver, string leftOn)
        {
            Id = Guid.NewGuid();
            ExternalRefId = externalRefId;
            PersonalCode = personalCode;
            Title = title;
            KnownAs = knownAs;
            Surname = surname;
            Fullname = fullname;
            PostNominals = postNominals;
            Email = email;
            CreatedDate = _currentUTC;
            UpdatedDate = _currentUTC;
            HasLeft = hasLeft;
            Leaver = leaver;
            LeftOn = leftOn;
        }
        
        public string ExternalRefId { get; set; }
        public string PersonalCode { get; set; }
        public string Title { get; set; }
        public string KnownAs { get; set; }
        public string Surname { get; set; }
        public string Fullname { get; set; }
        public string PostNominals { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string HasLeft { get; set; }
        public string Leaver { get; set; }
        public string LeftOn { get; set; }
    }
}