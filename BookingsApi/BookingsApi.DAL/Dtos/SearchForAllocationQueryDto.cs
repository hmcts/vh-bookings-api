using System;

namespace BookingsApi.DAL.Dtos;

public class SearchForAllocationQueryDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid[] Cso { get; set; }
    public string[] CaseType { get; set; }
    public string CaseNumber { get; set; }
    public bool IsUnallocated { get; set; }
}

public class HearingAllocationResultDto
{
    
}