using System.Net;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("JobHistory")]
    [ApiController]
    public class JobHistoryController : Controller
    {
        private readonly ICommandHandler _commandHandler;
        
        public JobHistoryController(ICommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }
        /// <summary>
        /// Insert a new record into the job history table
        /// </summary>
        /// <param name="jobName">The name of the job</param>
        /// <param name="isSuccessful">Did the job execute successfully</param>
        /// <returns>Details of the newly booked hearing</returns>
        [HttpPost("{jobName}/{isSuccessful:bool}")]
        [OpenApiOperation("UpdateJobHistory")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateJobHistory(string jobName, bool isSuccessful)
        {
            await _commandHandler.Handle(new AddJobHistoryCommand{JobName = jobName, IsSuccessful = true});
            return Ok();
        }
    }
}