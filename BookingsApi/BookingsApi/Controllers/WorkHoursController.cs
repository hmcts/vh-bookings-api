using BookingsApi.Contract.Requests;
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
    [Route("work-hours")]
    [ApiController]
    public class WorkHoursController : Controller
    {
        private readonly ICommandHandler _commandHandler;

        public WorkHoursController(ICommandHandler commandHandler)
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
        public async Task<IActionResult> SaveWorkHours([FromBody] List<UploadWorkHoursRequest> uploadWorkHoursRequests)
        {

            var validationResult = new UploadWorkHoursRequestsValidation().ValidateRequests(uploadWorkHoursRequests);

            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return BadRequest(ModelState);
            }

            var uploadWorkHoursCommand = new UploadWorkHoursCommand(uploadWorkHoursRequests);

            await _commandHandler.Handle(uploadWorkHoursCommand);

            return Ok(uploadWorkHoursCommand.FailedUploadUsernames);
        }


        /// <summary>
        /// Save vho non-working hours
        /// </summary>
        /// <param name="uploadNonWorkingHoursRequests"></param>
        /// <returns>List of usernames that were not found</returns>
        [HttpPost("SaveNonWorkingHours")]
        [OpenApiOperation("SaveNonWorkingHours")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveNonWorkingHours([FromBody] List<UploadNonWorkingHoursRequest> uploadNonWorkingHoursRequests)
        {

            var validationResult = new UploadNonWorkingHoursRequestsValidation().ValidateRequests(uploadNonWorkingHoursRequests);

            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return BadRequest(ModelState);
            }

            var uploadNonWorkingHoursCommand = new UploadNonWorkingHoursCommand(uploadNonWorkingHoursRequests);

            await _commandHandler.Handle(uploadNonWorkingHoursCommand);

            return Ok(uploadNonWorkingHoursCommand.FailedUploadUsernames);
        }
    }
}