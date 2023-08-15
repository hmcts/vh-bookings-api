namespace BookingsApi.DAL.Dtos;

public record WorkHoursDto(int DayOfWeekId, int? StartTimeHour, int? StartTimeMinutes, int? EndTimeHour, int? EndTimeMinutes)
{
    public TimeSpan? StartTime => StartTimeHour == null || StartTimeMinutes == null ? null : new TimeSpan((int)StartTimeHour, (int)StartTimeMinutes, 0);
        
    public TimeSpan? EndTime => EndTimeHour == null || EndTimeMinutes == null ? null : new TimeSpan((int)EndTimeHour, (int)EndTimeMinutes, 0);
}

public record AddNonWorkHoursDto(string Username, DateTime StartTime, DateTime EndTime);

public record UploadWorkHoursDto(string Username, List<WorkHoursDto> WorkingHours);

public record NonWorkHoursDto(long Id, DateTime StartTime, DateTime EndTime);