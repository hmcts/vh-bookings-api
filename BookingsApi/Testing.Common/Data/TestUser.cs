using BookingsApi.Contract.Requests;

namespace Testing.Common.Data
{
    public class TestUser : ParticipantRequest
    {
        public TestUser(string firstName, string lastName)
        {
            var fullFirstName = TestUsers.FirstNamePrefix + firstName;
            
            FirstName = fullFirstName;
            LastName = lastName;
            ContactEmail = $"{fullFirstName}_{lastName}@hmcts.net";
            Username = $"{fullFirstName}_{lastName}@hearings.reform.hmcts.net";
            DisplayName = $"{fullFirstName} {lastName}";
        }
    }
}
