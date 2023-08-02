using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("JobHistory")]
    [ApiVersion("1.0")]
    [ApiController]
    public class JobHistoryController : Controller
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;

        public JobHistoryController(ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }
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
            await _commandHandler.Handle(new AddJobHistoryCommand {JobName = jobName, IsSuccessful = true});
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
            var jobHistories = await _queryHandler.Handle<GetJobHistoryByJobNameQuery, List<JobHistory>>(new GetJobHistoryByJobNameQuery(jobName));   
            var obj = jobHistories.Select(e => new JobHistoryResponse(e.JobName, e.LastRunDate, e.IsSuccessful)).ToList();
            return Ok(obj);
        }
    }
}