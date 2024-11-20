using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("JobHistory")]
    [ApiVersion("1.0")]
    [ApiController]
    public class JobHistoryController(ICommandHandler commandHandler, IQueryHandler queryHandler) : ControllerBase
    {
        /// <summary>
        /// Insert a new record into the job history table
        /// </summary>
        /// <param name="jobName">The name of the job</param>
        /// <param name="isSuccessful">Did the job execute successfully</param>
        /// <returns>Details of the newly booked hearing</returns>
        [HttpPost("{jobName}/{isSuccessful:bool}")]
        [OpenApiOperation("UpdateJobHistory")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateJobHistory(string jobName, bool isSuccessful)
        {
            await commandHandler.Handle(new AddJobHistoryCommand {JobName = jobName, IsSuccessful = true});
            return NoContent();
        }

        /// <summary>
        /// Get JobHistory For Selected Job
        /// </summary>
        /// <param name="jobName">The name of the job</param>
        /// <returns>Job history list</returns>
        [HttpGet("{jobName}")]
        [OpenApiOperation("GetJobHistory")]
        [ProducesResponseType(typeof(List<JobHistoryResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetJobHistory(string jobName)
        {
            var jobHistories = await queryHandler.Handle<GetJobHistoryByJobNameQuery, List<JobHistory>>(new GetJobHistoryByJobNameQuery(jobName));   
            var obj = jobHistories.Select(e => new JobHistoryResponse(e.JobName, e.LastRunDate, e.IsSuccessful)).ToList();
            return Ok(obj);
        }
    }
}