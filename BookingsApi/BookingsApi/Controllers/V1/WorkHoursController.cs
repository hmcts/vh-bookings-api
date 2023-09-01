using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations;
using BookingsApi.Validations.V1;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("work-hours")]
    [ApiVersion("1.0")]
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
        [ProducesResponseType(typeof(ValidationProblemDetails), (int) HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SaveWorkHours([FromBody] List<UploadWorkHoursRequest> uploadWorkHoursRequests)
        {

            var validationResult = new UploadWorkHoursRequestsValidation().ValidateRequests(uploadWorkHoursRequests);

            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return ValidationProblem(ModelState);
            }

            
            var dto = uploadWorkHoursRequests.Select(request => 
                new UploadWorkHoursDto(
                    request.Username,
                    request.WorkingHours.Select(workingHours => 
                            new WorkHoursDto(workingHours.DayOfWeekId, 
                                workingHours.StartTimeHour, workingHours.StartTimeMinutes, 
                                workingHours.EndTimeHour, workingHours.EndTimeMinutes)).ToList()))
                .ToList();
            var uploadWorkHoursCommand = new UploadWorkHoursCommand(dto);

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
        [ProducesResponseType(typeof(List<string>), (int) HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SaveNonWorkingHours(
            [FromBody] List<UploadNonWorkingHoursRequest> uploadNonWorkingHoursRequests)
        {

            var validationResult =
                new UploadNonWorkingHoursRequestsValidation().ValidateRequests(uploadNonWorkingHoursRequests);

            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return BadRequest(ModelState);
            }

            var dto = uploadNonWorkingHoursRequests.Select(request =>
                new AddNonWorkHoursDto(request.Username, request.StartTime, request.EndTime)).ToList();
            var uploadNonWorkingHoursCommand = new UploadNonWorkingHoursCommand(dto);

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
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetVhoWorkAvailabilityHours(string username)
        {
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                return BadRequest(ModelState);
            }
            
            var results = await _queryHandler.Handle<GetVhoWorkHoursQuery, List<VhoWorkHours>>(new GetVhoWorkHoursQuery(username));

            if (results == null)
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
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetVhoNonAvailabilityHours(string username)
        {
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                return BadRequest(ModelState);
            }

            var results = await _queryHandler.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(new GetVhoNonAvailableWorkHoursQuery(username));

            if (results == null)
                return NotFound("Vho user not found");
            
            return Ok(VhoNonAvailabilityWorkHoursResponseMapper.Map(results));
        }
        
        /// <summary>
        /// Updates non availability hours for a vho
        /// </summary>
        /// <param name="username"></param>
        /// <param name="request"></param>
        /// <returns>Success status</returns>
        [HttpPatch("/NonAvailability/VHO/{username}")]
        [OpenApiOperation("UpdateVhoNonAvailabilityHours")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateVhoNonAvailabilityHours(string username, UpdateNonWorkingHoursRequest request)
        {
            var query = new GetJusticeUserByUsernameQuery(username);
            var user = await _queryHandler.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(query);

            if (user == null)
            {
                return NotFound();
            }
            
            var userId = user.Id;
            
            var validationResult = await new UpdateNonWorkingHoursRequestValidation().ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return BadRequest(ModelState);
            }
            var getNonWorkHoursQuery = new GetVhoNonAvailableWorkHoursQuery(username);
            var existingHours = await _queryHandler.Handle<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>(getNonWorkHoursQuery);
            if (existingHours == null)
            {
                return NotFound();
            }
            
            var nonDeletedHours = existingHours.Where(x => !x.Deleted).ToList();
            var hourValidationResult = new UpdateNonWorkingHoursRequestValidation().ValidateHours(request, nonDeletedHours);
            if (!hourValidationResult.IsValid)
            {
                if (hourValidationResult.Errors.Exists(x => x.ErrorMessage.Contains(UpdateNonWorkingHoursRequestValidation.HourIdsNotFoundErrorMessage)))
                {
                    return NotFound();
                }
                
                ModelState.AddFluentValidationErrors(hourValidationResult.Errors);
                return BadRequest(ModelState);
            }
            
            var dto = request.Hours.Select(r => new NonWorkHoursDto(r.Id, r.StartTime, r.EndTime)).ToList();
            var updateNonWorkingHoursCommand = new UpdateNonWorkingHoursCommand(userId, dto);
            await _commandHandler.Handle(updateNonWorkingHoursCommand);
            
            return NoContent();
        }

        /// <summary>
        /// Delete non availability work hours for vho
        /// </summary>
        /// <param name="username">the justice user username</param>
        /// <param name="nonAvailabilityId">Identifier for the non-available period</param>
        /// <returns>vho with list of non availability work hours</returns>
        [HttpDelete("/NonAvailability/VHO/{username}/{nonAvailabilityId}")]
        [OpenApiOperation("DeleteVhoNonAvailabilityHours")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails),(int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string),(int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteVhoNonAvailabilityHours([FromRoute] string username,
            [FromRoute] long nonAvailabilityId)
        {
            try
            {
                if (!username.IsValidEmail())
                {
                    ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                }

                if (nonAvailabilityId <= 0)
                {
                    ModelState.AddModelError(nameof(nonAvailabilityId),
                        $"Please provide a valid {nameof(nonAvailabilityId)}");
                }

                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }

                var deleteNonWorkingHoursCommand = new DeleteNonWorkingHoursCommand(username, nonAvailabilityId);
                await _commandHandler.Handle(deleteNonWorkingHoursCommand);
                return Ok();
            }
            catch (JusticeUserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (NonWorkingHoursNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}