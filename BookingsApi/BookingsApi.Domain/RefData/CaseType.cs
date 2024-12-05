using System;
using System.Collections.Generic;

namespace BookingsApi.Domain.RefData
{
    public class CaseType : TrackableEntity<int>
    {
        public const string CacdServiceId = "VIHTMP1"; // Court of Appeal Criminal Division
        public const string CccServiceId = "VIHTMP8"; // Crime Crown Court
        public CaseType(int id, string name)
        {
            Id = id;
            Name = name;
            Live = true;
        }
        
        public string Name { get; set; }
        
        public string ServiceId { get; set; }
        public bool Live { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public bool SupportsAudioRecording()
        {
            return ServiceId != CccServiceId && ServiceId != CacdServiceId;
        }
        
    }
}