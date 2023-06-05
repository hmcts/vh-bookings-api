using BookingsApi.Contract.Requests;
namespace Testing.Common.Data
{
    public class TestUser : ParticipantRequest
    {
        public TestUser(string firstName, string lastName)
        {
            FirstName = TestUsers.FirstNamePrefix + firstName;
            LastName = lastName;
            ContactEmail = $"{firstName}_{lastName}@hmcts.net";
            Username = $"{firstName}_{lastName}@hearings.reform.hmcts.net";
            DisplayName = $"{firstName} {lastName}";
        }
    }
}
