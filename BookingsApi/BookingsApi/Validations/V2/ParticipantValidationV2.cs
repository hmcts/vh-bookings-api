namespace BookingsApi.Validations.V2
{
    public static class ParticipantValidationV2
    {
        public const string NameRegex = "^(\\w+(?:\\w|[\\s'._-](?![\\s'._-]))*\\w+)$";
        public static readonly string FirstNameDoesntMatchRegex = "First name must match regular expression";
        public static readonly string LastNameDoesntMatchRegex = "Last name must match regular expression";
        public static readonly string NoFirstNameErrorMessage = "First name is required";
        public static readonly string NoLastNameErrorMessage = "Last name is required";
    }
}
