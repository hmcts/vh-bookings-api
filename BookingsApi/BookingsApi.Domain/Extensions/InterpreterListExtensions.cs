using System.Collections.Generic;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain.Extensions
{
    public static class InterpreterListExtensions
    {
        public static InterpreterLanguage GetLanguage(this List<InterpreterLanguage> languages, string languageCode, string errorKey = "Participant")
        {
            if (string.IsNullOrWhiteSpace(languageCode)) return null;
            var language = languages.Find(x=> x.Code == languageCode);

            if (language == null)
            {
                throw new DomainRuleException(errorKey, $"Language code {languageCode} does not exist");
            }
            
            return language;
        }
    }
}
