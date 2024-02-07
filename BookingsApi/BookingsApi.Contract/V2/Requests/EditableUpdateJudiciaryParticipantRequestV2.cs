namespace BookingsApi.Contract.V2.Requests
{
    public class EditableUpdateJudiciaryParticipantRequestV2 : UpdateJudiciaryParticipantRequestV2
    {
        /// <summary>
        /// The participant's judicial personal code
        /// </summary>
        public string PersonalCode { get; set; }
    }
}
