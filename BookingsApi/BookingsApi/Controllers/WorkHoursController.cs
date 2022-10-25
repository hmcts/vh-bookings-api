using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.Extensions;
using BookingsApi.Validations;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
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
        
        [HttpPatch("/NonAvailability/VHO")]
        [OpenApiOperation("UpdateVhoNonAvailabilityHours")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateVhoNonAvailabilityHours(UpdateNonWorkingHoursRequest request)
        {
            // TODO refactor validation - see existing Validation classes that do this

            if (request.Hours == null || !request.Hours.Any())
            {
                ModelState.AddModelError("Hours", "Hours cannot be null or empty");
                return BadRequest(ModelState);
            }
            
            int i = 0;
            
            foreach (var hour in request.Hours)
            {
                if (hour.EndTime <= hour.StartTime)
                {
                    ModelState.AddModelError($"Hours[{i}].EndTime", "EndTime must be after StartTime");
                }

                i++;
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(ModelState);
            }
            
            var getNonWorkHoursByIdsQuery = new GetVhoNonAvailableWorkHoursByIdsQuery(request.Hours.Select(h => h.Id).ToList());
            var workHours = await _queryHandler.Handle<GetVhoNonAvailableWorkHoursByIdsQuery, List<VhoNonAvailability>>(getNonWorkHoursByIdsQuery);

            if (workHours == null || !workHours.Any())
            {
                return NotFound();
            }
            
            var requestedWorkHourIds = request.Hours.Select(h => h.Id).ToList();
            var foundWorkHourIds = workHours.Select(h => h.Id).ToList();

            if (!requestedWorkHourIds.All(foundWorkHourIds.Contains))
            {
                return NotFound();
            }

            var newWorkHours = workHours.ToList();
            foreach (var newWorkHour in newWorkHours)
            {
                var requestedHour = request.Hours.SingleOrDefault(h => h.Id == newWorkHour.Id);

                newWorkHour.StartTime = requestedHour.StartTime;
                newWorkHour.EndTime = requestedHour.EndTime;
            }
            
            // TODO check if dates overlap for a single user
            var userIds = newWorkHours.Select(h => h.JusticeUserId)
                .Distinct()
                .ToList();

            foreach (var userId in userIds)
            {
                var hoursForUser = newWorkHours
                    .Where(h => h.JusticeUserId == userId)
                    .OrderBy(h => h.StartTime)
                    .ToList();

                var first = (VhoNonAvailability)null;
                var checkedHours = new List<VhoNonAvailability>();
    
                foreach (var hour in hoursForUser)
                {
                    if (first != null)
                    {
                        checkedHours.Add(first);
                        var uncheckedHours = hoursForUser.Where(x => (x.StartTime >= first.StartTime && !(x == first)) && !checkedHours.Any(m => m == x));
            
                        foreach (var uncheckedHour in uncheckedHours)
                        {
                            if (OverlapsWith(first, uncheckedHour))
                            {
                                ModelState.AddModelError("Hours", "Hours cannot overlap for a single user");
                                //yield return new[] { first, meet };
                                break;
                            }
                        }
                    }
                    first = hour;

                    bool OverlapsWith(VhoNonAvailability first, VhoNonAvailability second)
                    {
                        return first.EndTime > second.StartTime;
                    }
                }
            }
            
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(ModelState);
            }

            var updateNonWorkingHoursCommand = new UpdateNonWorkingHoursCommand(request.Hours);
            await _commandHandler.Handle(updateNonWorkingHoursCommand);
            
            return NoContent();
        }
    }
}