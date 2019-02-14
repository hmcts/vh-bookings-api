using System.Collections.Generic;
using System.Linq;
using Bookings.Domain;
using Bookings.Domain.Enumerations;

namespace Testing.Common.Builders.Domain
{
    public class ChecklistBuilder
    {
        private readonly List<ChecklistQuestion> _questions = new List<ChecklistQuestion>();

        public ChecklistBuilder AddQuestion(Role role, string key)
        {
            var question = new ChecklistQuestion(role) {
                Key = key
            };

            _questions.Add(question);
            return this;
        }

        public List<ChecklistQuestion> BuildQuestions()
        {
            return _questions.ToList();
        }
    }
}