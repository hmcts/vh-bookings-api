using BookingsApi.Contract.V2.Requests;

namespace Testing.Common.Data
{
    public class TestUser : ParticipantRequestV2
    {
        public TestUser(string firstName, string lastName)
        {
            var fullFirstName = TestUsers.FirstNamePrefix + firstName;
            
            FirstName = fullFirstName;
            LastName = lastName;
            ContactEmail = $"{fullFirstName}_{lastName}@hmcts.net".ToLower();
            DisplayName = $"{fullFirstName} {lastName}";
        }
    }
}
