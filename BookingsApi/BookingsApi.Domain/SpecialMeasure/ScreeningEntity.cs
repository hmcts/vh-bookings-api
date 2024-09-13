// using System;
// using BookingsApi.Domain.Participants;
//
// namespace BookingsApi.Domain.SpecialMeasure;
//
// public class ScreeningEntity : TrackableEntity<long>
// {
//     public Guid ScreeningId { get; set; }
//     public virtual Screening Screening { get; set; }
//     
//     public Guid? ParticipantId { get; set; }
//     public virtual Participant Participant { get; set; }
//     
//     public Guid? EndpointId { get; set; }
//     public virtual Endpoint Endpoint { get; set; }
// }