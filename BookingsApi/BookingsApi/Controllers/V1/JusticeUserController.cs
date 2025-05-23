using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations.V1;
using BookingsApi.Common.Logging;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("justiceuser")]
    [ApiVersion("1.0")]
    [ApiController]
    public class JusticeUserController(
        IQueryHandler queryHandler,
        ICommandHandler commandHandler,
        ILogger<JusticeUserController> logger)
        : ControllerBase
    {
        /// <summary>
        /// Add a new justice user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [OpenApiOperation("AddJusticeUser")]
        [ProducesResponseType(typeof(JusticeUserResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddJusticeUser(AddJusticeUserRequest request)
        {
            var validation = await new AddJusticeUserRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }

            var userRoleIds = request.Roles.Select(x => (int)x).ToArray();
            var command = new AddJusticeUserCommand(request.FirstName, request.LastName, request.Username,
                request.ContactEmail, request.CreatedBy, userRoleIds)
            {
                Telephone = request.ContactTelephone
            };
            try
            {
                await commandHandler.Handle(command);
                var justiceUser =
                    await queryHandler.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(
                        new GetJusticeUserByUsernameQuery(request.Username));

                var justiceUserResponse = JusticeUserToResponseMapper.Map(justiceUser);
                return CreatedAtAction(actionName: nameof(GetJusticeUserByUsername),
                    routeValues: new { username = request.Username },
                    value: justiceUserResponse);
            }
            catch (JusticeUserAlreadyExistsException e)
            {
                logger.LogErrorDetectedAnExisting(e, request.Username);
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
        [ProducesResponseType(typeof(JusticeUserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> EditJusticeUser(EditJusticeUserRequest request)
        {
            var validation = await new EditJusticeUserRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }

            var userRoleIds = request.Roles?.Select(x => (int)x).ToArray();
            var command = new EditJusticeUserCommand(request.Id, request.Username, request.FirstName, request.LastName,
                request.ContactTelephone, userRoleIds);

            await commandHandler.Handle(command);
            var justiceUser =
                await queryHandler.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(
                    new GetJusticeUserByUsernameQuery(request.Username));

            var justiceUserResponse = JusticeUserToResponseMapper.Map(justiceUser);
            return Ok(justiceUserResponse);

        }

        /// <summary>
        /// Find justice user with matching username.
        /// </summary>
        /// <param name="username">String to match the username with.</param>
        /// <returns>Person list</returns>
        [HttpGet("GetJusticeUserByUsername")]
        [OpenApiOperation("GetJusticeUserByUsername")]
        [ProducesResponseType(typeof(JusticeUserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetJusticeUserByUsername(string username)
        {
            var query = new GetJusticeUserByUsernameQuery(username);
            var justiceUser =
                await queryHandler.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(query);

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
        [ProducesResponseType(typeof(List<JusticeUserResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetJusticeUserList(string term, bool includeDeleted = false)
        {
            var query = new GetJusticeUserListQuery(term, includeDeleted);
            var userList =
                await queryHandler.Handle<GetJusticeUserListQuery, List<JusticeUser>>(query);

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
        public async Task<IActionResult> DeleteJusticeUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError(nameof(id), $"Please provide a valid {nameof(id)}");
                return ValidationProblem();
            }

            var command = new DeleteJusticeUserCommand(id);
            await commandHandler.Handle(command);
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
        public async Task<IActionResult> RestoreJusticeUser(RestoreJusticeUserRequest request)
        {
            var validation = await new RestoreJusticeUserRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }

            var command = new RestoreJusticeUserCommand(request.Id);

            await commandHandler.Handle(command);
            return NoContent();

        }
    }
}