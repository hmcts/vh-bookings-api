//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Bookings.Domain.Enumerations;
//using HearingStatus = Bookings.Domain.Enumerations.HearingStatus;
//
// TODO: review this
//namespace Bookings.Domain
//{
//    public class HearingStateMachine
//    {
//        private readonly List<KeyValuePair<Enumerations.HearingStatus, StateTransition>> _stateTransitions;
//
//        public HearingStateMachine()
//        {
//            _stateTransitions = new List<KeyValuePair<Enumerations.HearingStatus, StateTransition>>
//            {
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Created, new StateTransition(HearingStatus.Live, new List<Role> {Role.Judge, Role.Administrator})),
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Live, new StateTransition(HearingStatus.Paused, new List<Role> {Role.Judge, Role.Administrator})),
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Live, new StateTransition(HearingStatus.Closed, new List<Role> {Role.Judge, Role.Administrator})),
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Live, new StateTransition(HearingStatus.Adjourned, new List<Role> {Role.Judge, Role.Administrator})),
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Live, new StateTransition(HearingStatus.Suspended, new List<Role> {Role.Judge, Role.Administrator})),
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Paused, new StateTransition(HearingStatus.Live, new List<Role> {Role.Judge, Role.Administrator})),
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Paused, new StateTransition(HearingStatus.Closed, new List<Role> {Role.Judge, Role.Administrator})),
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Adjourned, new StateTransition(HearingStatus.Closed, new List<Role> {Role.Judge, Role.Administrator})),
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Adjourned, new StateTransition(HearingStatus.Suspended, new List<Role> {Role.Judge, Role.Administrator})),
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Suspended, new StateTransition(HearingStatus.Closed, new List<Role> {Role.Judge, Role.Administrator})),
//                new KeyValuePair<Enumerations.HearingStatus, StateTransition>(HearingStatus.Suspended, new StateTransition(HearingStatus.Live, new List<Role> {Role.Judge, Role.Administrator}))
//            };
//        }
//
//        public bool ValidateStatusChange(StatusChangedEvent statusChangedEvent)
//        { 
//            var transition = _stateTransitions.SingleOrDefault(x => x.Key == statusChangedEvent.CurrentStatus && x.Value.NewStatus == statusChangedEvent.NewStatus).Value;
//            return transition != null && transition.ParticipantRoles.Contains(statusChangedEvent.ParticipantRole);
//        }
//    }
//
//    public class StateTransition
//    {
//        public Enumerations.HearingStatus NewStatus { get; }
//        public IList<Role> ParticipantRoles { get; }
//
//        public StateTransition(Enumerations.HearingStatus newStatus, IList<Role> participantRoles)
//        {
//            NewStatus = newStatus;
//            ParticipantRoles = participantRoles;
//        }
//
//        public override int GetHashCode()
//        {
//            return 17 + 31 * NewStatus.GetHashCode() + 31 * ParticipantRoles.GetHashCode();
//        }
//
//        public override bool Equals(object obj)
//        {
//            if (obj is StateTransition stateTransition)
//            {
//                return stateTransition.NewStatus == this.NewStatus &&
//                       stateTransition.ParticipantRoles.Equals(this.ParticipantRoles);
//            }
//
//            return false;
//        }
//    }
//
//}