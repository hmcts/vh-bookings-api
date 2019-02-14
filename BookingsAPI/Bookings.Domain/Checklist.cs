using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Domain.Enumerations;
using Bookings.Domain.Participants;
using Bookings.Domain.Validations;

namespace Bookings.Domain
{
    /// <summary>
    /// Represents a checklist of questions a participant of a specific role must answer before a hearing
    /// </summary>
    public class Checklist
    {
        private readonly List<ChecklistQuestion> _questions;
        private readonly Participant _participant;
        private readonly List<ChecklistAnswer> _answers;

        protected Checklist(Participant participant, IEnumerable<ChecklistQuestion> questions, IEnumerable<ChecklistAnswer> answers)
        {
            _participant = participant;
            _questions = questions.ToList();
            _questions.ForEach(ValidateQuestionRole);
            _answers = answers.ToList();
        }

        public IEnumerable<ChecklistAnswer> Answers => _answers;
        public IEnumerable<ChecklistQuestion> Questions => _questions;

        /// <summary>
        /// True if there are questions that must be answered by the participant
        /// </summary>
        public bool IsRequiredForHearing(Hearing hearing)
        {
            var hearingIsPending = hearing.Status != HearingStatus.Closed &&
                                   hearing.ScheduledDateTime >= DateTime.UtcNow.Date;

            return _questions.Any() && !IsSubmitted && hearingIsPending;
        }

        public bool IsSubmitted => _answers.Count > 1;

        private void ValidateQuestionRole(ChecklistQuestion question)
        {
            if (question.Role.ToString() == _participant.HearingRole.UserRole.Name)
                return;

            throw new DomainRuleException(nameof(Checklist), $"Question does not belong to role '{_participant.HearingRole.UserRole.Name}': {question.Key}");
        }

        public void Answer(string questionKey, string answer, string notes)
        {
            if (_answers.Any(x => x.Question.Key == questionKey))
            {
                throw new DomainRuleException(nameof(Checklist),
                    $"An answer to question '{questionKey}' has already been set");
            }

            _answers.Add(new ChecklistAnswer(GetQuestion(questionKey))
            {
                Answer = answer,
                Notes = notes
            });
        }

        private ChecklistQuestion GetQuestion(string questionKey)
        {
            var question = _questions.FirstOrDefault(q => q.Key == questionKey);
            if (question == null)
            {
                throw new DomainRuleException(nameof(Checklist),
                    $"No question with key '{questionKey}' exists for role '{_participant.HearingRole}'");
            }

            return question;
        }

        /// <summary>
        /// Creates a brand new checklist ignoring any existing answers
        /// </summary>
        public static Checklist New(Participant participant, IEnumerable<ChecklistQuestion> questions)
        {
            return new Checklist(participant, questions, new List<ChecklistAnswer>());
        }

        /// <summary>
        /// Creates a checklist containing the answers put in by the participant already
        /// </summary>
        public static Checklist Existing(Participant participant, IEnumerable<ChecklistQuestion> questions)
        {
            return new Checklist(participant, questions, participant.GetChecklistAnswers());
        }
    }
}
