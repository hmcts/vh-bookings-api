namespace BookingsApi.Contract.V1.Requests
{
    public class EditableUpdateJudiciaryParticipantRequest: UpdateJudiciaryParticipantRequest
    {
        /// <summary>
        /// The participant's judicial personal code
        /// </summary>
        public string PersonalCode { get; set; }
    }
}
