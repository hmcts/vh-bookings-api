using System;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.Extensions;
using BookingsApi.Validations;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Mappings;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("work-hours")]
    [ApiController]
    public class WorkHoursController : Controller
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;

        public WorkHoursController(ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
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
                
        /// <summary>
        /// Search for a vho and return with availability work hours
        /// </summary>
        /// <param name="username"></param>
        /// <returns>vho with list of availability work hours</returns>
        [HttpGet("VHO")]
        [OpenApiOperation("GetVhoWorkAvailabilityHours")]
        [ProducesResponseType(typeof(List<VhoWorkHoursResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetVhoWorkAvailabilityHours(string username)
        {
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                return BadRequest(ModelState);
            }

            var results = await _queryHandler.Handle<GetVhoWorkHoursQuery, List<VhoWorkHours>>(new GetVhoWorkHoursQuery(username));

            if (results == null || !results.Any())
                return NotFound("Vho user not found");
            
            return Ok(VhoWorkHoursToResponseMapper.Map(results));
        }
        
        /// <summary>
        /// Search for a vho and return with non availability work hours
        /// </summary>
        /// <param name="username"></param>
        /// <returns>vho with list of non availability work hours</returns>
        [HttpGet("/NonAvailability/VHO")]
        [OpenApiOperation("GetVhoNonAvailabilityHours")]
        [ProducesResponseType(typeof(List<VhoNonAvailabilityWorkHoursResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetVhoNonAvailabilityHours(string username)
        {
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                return BadRequest(ModelState);
            }

            var results = await _queryHandler.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(new GetVhoNonAvailableWorkHoursQuery(username));

            if (results == null || !results.Any())
                return NotFound("Vho user not found");
            
            return Ok(VhoNonAvailabilityWorkHoursResponseMapper.Map(results));
        }
        
        /// <summary>
        /// Search for a vho and return with non availability work hours
        /// </summary>
        /// <param name="username"></param>
        /// <returns>vho with list of non availability work hours</returns>
        [HttpDelete("/NonAvailability")]
        [OpenApiOperation("DeleteVhoNonAvailabilityHours")]
        [ProducesResponseType(typeof(List<VhoNonAvailabilityWorkHoursResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteVhoNonAvailabilityHours(long id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError(nameof(id), $"Please provide a valid {nameof(id)}");
                return BadRequest(ModelState);
            }

            var deleteNonWorkingHoursCommand = new DeleteNonWorkingHoursCommand(id);

            await _commandHandler.Handle(deleteNonWorkingHoursCommand);
            
            return Ok();
        }
    }
}