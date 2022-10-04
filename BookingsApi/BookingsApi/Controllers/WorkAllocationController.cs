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
        /// Save vho work schedule
        /// </summary>
        /// <param name="uploadWorkAllocationRequests"></param>
        /// <returns>List of usernames that were not found</returns>
        [HttpPost("SaveWorkAllocations")]
        [OpenApiOperation("SaveWorkAllocations")]
        [ProducesResponseType(typeof(List<string>), (int) HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<IActionResult> SaveWorkAllocations([FromBody] List<UploadWorkAllocationRequest> uploadWorkAllocationRequests)
        {

            var validationResult = new UploadWorkAllocationRequestsValidation().ValidateRequests(uploadWorkAllocationRequests);

            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return BadRequest(ModelState);
            }

            var uploadWorkAllocationCommand = new UploadWorkHoursCommand(uploadWorkAllocationRequests);

            await _commandHandler.Handle(uploadWorkAllocationCommand);

            return Ok(uploadWorkAllocationCommand.FailedUploadUsernames);
        }
    }
}