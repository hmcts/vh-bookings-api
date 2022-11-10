namespace BookingsApi.Common.Configuration
{
    public class AllocateCsoConfiguration
    {
        public bool AllowHearingToStartBeforeWorkStartTime { get; set; }
        public bool AllowHearingToEndAfterWorkEndTime { get; set; }
        public int MinimumGapBetweenHearingsInMinutes { get; set; }
        public int MaximumConcurrentHearings { get; set; }
    }
}
