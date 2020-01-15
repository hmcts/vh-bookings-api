﻿using Bookings.Domain.Ddd;
using System;

namespace Bookings.Domain
{
    public class SuitabilityAnswer : Entity<long>
    {
        public SuitabilityAnswer(string key, string data, string extendedData)
        {
            Key = key;
            Data = data;
            ExtendedData = extendedData;
            UpdatedDate = DateTime.UtcNow;
        }

        public long QuestionnaireId { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
        public string ExtendedData { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Questionnaire Questionnaire { get; set; }
    }
}
