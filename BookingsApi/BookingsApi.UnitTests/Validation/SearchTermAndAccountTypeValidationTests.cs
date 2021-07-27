using BookingsApi.Contract;
using BookingsApi.Validations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using BookingsApi.Contract.Requests;

namespace BookingsApi.UnitTests.Validation
{
    public class SearchTermAndAccountTypeValidationTests
    {
        private SearchTermAndAccountTypeRequestValidation _validator;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new SearchTermAndAccountTypeRequestValidation();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public async Task Returns_Search_Term_Error_When_Term_Is_Invalid(string searchTerm)
        {
            var request = new SearchTermAndAccountTypeRequest(searchTerm);

            var result = await _validator.ValidateAsync(request);

            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == SearchTermAndAccountTypeRequestValidation.SearchTermErrorMessage)
                .Should().BeTrue();
        }

        [Test]
        public async Task Returns_Specified_Account_Type_Error_When_Property_Is_Corrupted()
        {
            var request = new SearchTermAndAccountTypeRequest("something") { AccountType = null};
            
            var result = await _validator.ValidateAsync(request);

            result.Errors.Count.Should().Be(1);
            result.Errors.Any(x => x.ErrorMessage == SearchTermAndAccountTypeRequestValidation.AccountTypeErrorMessage)
                .Should().BeTrue();
        }

    }
}
