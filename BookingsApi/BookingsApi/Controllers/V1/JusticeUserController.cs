using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Commands.V1;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain;
using BookingsApi.Extensions;
using BookingsApi.Mappings;
using BookingsApi.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("justiceuser")]
    [ApiController]
    [ApiVersion("1.0")]
    public class JusticeUserController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<JusticeUserController> _logger;

        public JusticeUserController(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<JusticeUserController> logger)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _logger = logger;
        }

        /// <summary>
        /// Add a new justice user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [OpenApiOperation("AddJusticeUser")]
        [ProducesResponseType(typeof(JusticeUserResponse),(int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  AddJusticeUser(AddJusticeUserRequest request)
        {
            var validation = await new AddJusticeUserRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }       
            var userRoleIds = request.Roles.Select(x => (int) x).ToArray();
            var command = new AddJusticeUserCommand(request.FirstName, request.LastName, request.Username,
                request.ContactEmail, request.CreatedBy, userRoleIds)
            {
                Telephone = request.ContactTelephone
            };
            try
            {
                await _commandHandler.Handle(command);
                var justiceUser =
                    await _queryHandler.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(
                        new GetJusticeUserByUsernameQuery(request.Username));

                var justiceUserResponse = JusticeUserToResponseMapper.Map(justiceUser);
                return CreatedAtAction(actionName: nameof(GetJusticeUserByUsername),
                    routeValues: new {username = request.Username},
                    value: justiceUserResponse);
            }
            catch (JusticeUserAlreadyExistsException e)
            {
                _logger.LogError(e, "Detected an existing user for the username {Username}", request.Username);
                return Conflict(e.Message);
            }
        }

        /// <summary>
        /// Edit a justice user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch]
        [OpenApiOperation("EditJusticeUser")]
        [ProducesResponseType(typeof(JusticeUserResponse),(int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  EditJusticeUser(EditJusticeUserRequest request)
        {
            var validation = await new EditJusticeUserRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }
            int[] userRoleIds = request.Roles?.Select(x => (int) x).ToArray();
            var command = new EditJusticeUserCommand(request.Id, request.Username, userRoleIds);
            try
            {
                await _commandHandler.Handle(command);
                var justiceUser =
                    await _queryHandler.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(
                        new GetJusticeUserByUsernameQuery(request.Username));

                var justiceUserResponse = JusticeUserToResponseMapper.Map(justiceUser);
                return Ok(justiceUserResponse);
            }
            catch (JusticeUserNotFoundException e)
            {
                _logger.LogError(e, "User not found for the username {Username}", request.Username);
                return NotFound(e.Message);
            }
        }
        
        /// <summary>
        /// Find justice user with matching username.
        /// </summary>
        /// <param name="username">String to match the username with.</param>
        /// <returns>Person list</returns>
        [HttpGet("GetJusticeUserByUsername")]
        [OpenApiOperation("GetJusticeUserByUsername")]
        [ProducesResponseType(typeof(JusticeUserResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  GetJusticeUserByUsername(string username)
        {
            var query = new GetJusticeUserByUsernameQuery(username);
            var justiceUser =
                await _queryHandler.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(query);

            if (justiceUser == null) 
                return NotFound();

            var justiceUserResponse = JusticeUserToResponseMapper.Map(justiceUser);

            return Ok(justiceUserResponse);
        }

        /// <summary>
        /// Get a list of justice users. Optionally provide a search term to filter
        /// for users that contain the given term in their first name, contact email or username.
        /// </summary>
        /// <param name="term">term to filter result</param>
        /// <param name="includeDeleted">include soft-deleted justice users</param>
        /// <returns>Justice User list</returns>
        [HttpGet("GetJusticeUserList")]
        [OpenApiOperation("GetJusticeUserList")]
        [ProducesResponseType(typeof(List<JusticeUserResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  GetJusticeUserList(string term, bool includeDeleted = false)
        {
            var query = new GetJusticeUserListQuery(term, includeDeleted);
            var userList =
                await _queryHandler.Handle<GetJusticeUserListQuery, List<JusticeUser>>(query);

            return Ok(userList.Select(JusticeUserToResponseMapper.Map).ToList());
        }

        /// <summary>
        /// Delete a justice user
        /// </summary>
        /// <param name="id">The justice user id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [OpenApiOperation("DeleteJusticeUser")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  DeleteJusticeUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(nameof(id), $"Please provide a valid {nameof(id)}");
                return ValidationProblem();
            }

            var command = new DeleteJusticeUserCommand(id);

            try
            {
                await _commandHandler.Handle(command);
            }
            catch (JusticeUserNotFoundException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }
        
        /// <summary>
        /// Restore a justice user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("restore")]
        [OpenApiOperation("RestoreJusticeUser")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  RestoreJusticeUser(RestoreJusticeUserRequest request)
        {
            var validation = await new RestoreJusticeUserRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }

            var command = new RestoreJusticeUserCommand(request.Id);

            try
            {
                await _commandHandler.Handle(command);
                return NoContent();
            }
            catch (JusticeUserNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}