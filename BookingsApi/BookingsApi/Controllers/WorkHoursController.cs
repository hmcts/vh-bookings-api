using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.Extensions;
using BookingsApi.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("workallocation")]
    [ApiController]
    public class WorkAllocationController : Controller
    {
        private readonly ICommandHandler _commandHandler;

        public WorkAllocationController(ICommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Save vho work hours
        /// </summary>
        /// <param name="uploadWorkHoursRequests"></param>
        /// <returns>List of usernames that were not found</returns>
        [HttpPost("SaveWorkHours")]
        [OpenApiOperation("SaveWorkHours")]
        [ProducesResponseType(typeof(List<string>), (int) HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<IActionResult> SaveWorkHours([FromBody] List<UploadWorkHoursRequest> uploadWorkHoursRequests)
        {

            var validationResult = new UploadWorkHoursRequestsValidation().ValidateRequests(uploadWorkHoursRequests);

            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return BadRequest(ModelState);
            }

            var uploadWorkAllocationCommand = new UploadWorkHoursCommand(uploadWorkHoursRequests);

            await _commandHandler.Handle(uploadWorkAllocationCommand);

            return Ok(uploadWorkAllocationCommand.FailedUploadUsernames);
        }
    }
}