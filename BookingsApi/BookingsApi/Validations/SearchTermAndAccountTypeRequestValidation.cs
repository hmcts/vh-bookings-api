using BookingsApi.Contract.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.Validations
{
    public class SearchTermAndAccountTypeRequestValidation : AbstractValidator<SearchTermAndAccountTypeRequest>
    {
        public const string SearchTermErrorMessage = "Search term is required";
        public const string AccountTypeErrorMessage = "Account type should not be null";

        public SearchTermAndAccountTypeRequestValidation()
        {
            RuleFor(x => x.Term).NotEmpty().WithMessage(SearchTermErrorMessage);
            RuleFor(x => x.AccountType).NotNull().WithMessage(AccountTypeErrorMessage);
        }
    }
}
