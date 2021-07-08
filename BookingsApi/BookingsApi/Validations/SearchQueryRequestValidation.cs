using BookingsApi.Contract.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.Validations
{
    public class SearchQueryRequestValidation : AbstractValidator<SearchQueryRequest>
    {
        public const string TermErrorMessage = "Search term is required";
        public const string JudiciaryUsernamesFromAdErrorMessage = "List of judiciary usernames from list cannot be null or empty";

        public SearchQueryRequestValidation()
        {
            RuleFor(x => x.Term).NotEmpty().WithMessage(TermErrorMessage);
            RuleFor(x => x.JudiciaryUsernamesFromAd).NotEmpty().WithMessage(JudiciaryUsernamesFromAdErrorMessage);
        }
    }
}
