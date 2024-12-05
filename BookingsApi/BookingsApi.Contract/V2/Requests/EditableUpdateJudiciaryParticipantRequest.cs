namespace BookingsApi.Contract.V2.Requests
{
    public class EditableUpdateJudiciaryParticipantRequest : UpdateJudiciaryParticipantRequest
    {
        /// <summary>
        /// The participant's judicial personal code
        /// </summary>
        public string PersonalCode { get; set; }
    }
}
