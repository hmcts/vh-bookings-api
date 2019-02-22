using FluentValidation;
using Bookings.Api.Contract.Requests;

namespace Bookings.API.Validations
{
    public class PaginationValidator : AbstractValidator<PaginatedRequest>
    {
        public PaginationValidator(int maxPageSize)
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(1);
            RuleFor(x => x.PageSize).LessThan(maxPageSize);
        }
    }
}
