using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Common;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Extensions;
using BookingsApi.Validations;
using Microsoft.Extensions.Logging;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("justiceuser")]
    [ApiController]
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
        [OpenApiOperation("AddAJusticeUser")]
        [ProducesResponseType(typeof(JusticeUserResponse),(int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> AddAJusticeUser(AddJusticeUserRequest request)
        {
            var validation = await new AddJusticeUserRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }
            var command = new AddJusticeUserCommand(request.FirstName, request.LastName, request.Username,
                request.ContactEmail, request.CreatedBy, (int) request.Role)
            {
                Telephone = request.Telephone
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
        /// Find justice user with matching username.
        /// </summary>
        /// <param name="username">String to match the username with.</param>
        /// <returns>Person list</returns>
        [HttpGet("GetJusticeUserByUsername")]
        [OpenApiOperation("GetJusticeUserByUsername")]
        [ProducesResponseType(typeof(JusticeUserResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetJusticeUserByUsername(string username)
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
        /// <returns>Justice User list</returns>
        [HttpGet("GetJusticeUserList")]
        [OpenApiOperation("GetJusticeUserList")]
        [ProducesResponseType(typeof(List<JusticeUserResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetJusticeUserList(string term)
        {
            var query = new GetJusticeUserListQuery(term);
            var userList =
                await _queryHandler.Handle<GetJusticeUserListQuery, List<JusticeUser>>(query);
            
            var list = userList.Select(user => JusticeUserToResponseMapper.Map(user));

            return Ok(list.ToList());
        }

        /// <summary>
        /// Delete a justice user
        /// </summary>
        /// <param name="id">The justice user id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [OpenApiOperation("DeleteJusticeUser")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteJusticeUser(Guid id)
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
    }
}