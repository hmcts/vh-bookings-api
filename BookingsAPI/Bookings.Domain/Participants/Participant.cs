using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Domain.Ddd;
using Bookings.Domain.RefData;

namespace Bookings.Domain.Participants
{
    public abstract class Participant: Entity<Guid>
    {
        protected Participant()
        {
            Id = Guid.NewGuid();
            ChecklistAnswers = new List<ChecklistAnswer>();
            CreatedDate = DateTime.UtcNow;
        }

        protected Participant(Person person, HearingRole hearingRole, CaseRole caseRole) : this()
        {
            Person = person;
            PersonId = person.Id;
            HearingRoleId = hearingRole.Id;
            CaseRoleId = caseRole.Id;

        }

        public string DisplayName { get; set; }
        public int CaseRoleId { get; set; }
        public virtual CaseRole CaseRole { get; set; }
        public int HearingRoleId { get; set; }
        public virtual HearingRole HearingRole { get; set; }
        public Guid PersonId { get; protected set; }
        public virtual Person Person { get; protected set; }
        public Guid HearingId { get; protected set; }
        public virtual Hearing Hearing { get; protected set; }   
        public virtual IList<ChecklistAnswer> ChecklistAnswers { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public void UpdatePersonDetails(Person person)
        {
            PersonId = Person.Id;
            Person = person;
        }
        
        public void UpdateDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName)) return;
            DisplayName = displayName;
            UpdatedDate = DateTime.UtcNow;
        }

        public IEnumerable<ChecklistAnswer> GetChecklistAnswers()
        {
            return ChecklistAnswers.ToList();
        }

        public void UpdateChecklist(Checklist checklist)
        {
            ChecklistAnswers.Clear();
            foreach (var answer in checklist.Answers)
            {
                ChecklistAnswers.Add(answer);
            }
        }
    }
}