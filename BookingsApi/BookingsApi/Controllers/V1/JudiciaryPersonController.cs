using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Configuration;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Commands.V1;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain;
using BookingsApi.Mappings;
using BookingsApi.Services;
using BookingsApi.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("judiciaryperson")]
    [ApiController]
    [ApiVersion("1.0")]
    public class JudiciaryPersonController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<JudiciaryPersonController> _logger;
        private readonly IFeatureFlagService _flagsService;

        public JudiciaryPersonController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<JudiciaryPersonController> logger, IFeatureFlagService flagsService)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _logger = logger;
            _flagsService = flagsService;
        }

        [HttpPost("BulkJudiciaryPersons")]
        [OpenApiOperation("BulkJudiciaryPersons")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BulkJudiciaryPersonResponse), (int) HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  BulkJudiciaryPersonsAsync(IEnumerable<JudiciaryPersonRequest> request)
        {
            const string bulkItemErrorMessage = "Could not add or update external Judiciary user with Personal Code: {0}";
            var judiciaryPersonRequests = request.ToList();
                _logger.LogInformation(
                "Starting BulkJudiciaryPersons operation, processing {JudiciaryPersonRequestsCount} items",
                judiciaryPersonRequests.Count);

            var bulkResponse = new BulkJudiciaryPersonResponse();

            foreach (var item in judiciaryPersonRequests)
            {
                var validation = item.Leaver
                    ? await new JudiciaryLeaverPersonRequestValidation().ValidateAsync(item)
                    : await new JudiciaryNonLeaverPersonRequestValidation().ValidateAsync(item);
                if (!validation.IsValid)
                {
                    bulkResponse.ErroredRequests.Add(new JudiciaryPersonErrorResponse
                    {
                        Message =
                            $"{string.Format(bulkItemErrorMessage, item.PersonalCode)} - {string.Join(", ", validation.Errors.Select(x => x.ErrorMessage))}",
                        JudiciaryPersonRequest = item
                    });

                    continue;
                }

                try
                {
                    var query = new GetJudiciaryPersonByPersonalCodeQuery(item.PersonalCode);
                    var judiciaryPerson =
                        await _queryHandler.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(query);

                    if (judiciaryPerson == null)
                    {
                        await _commandHandler.Handle(new AddJudiciaryPersonByPersonalCodeCommand(item.Id,
                            item.PersonalCode, item.Title, item.KnownAs, item.Surname,
                            item.Fullname, item.PostNominals, item.Email, item.HasLeft, item.Leaver, item.LeftOn));
                    }
                    else
                    {
                        await _commandHandler.Handle(UpdateJudiciaryPersonByExternalRefIdCommandMapper.Map(item));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, bulkItemErrorMessage, item.PersonalCode);
                    bulkResponse.ErroredRequests.Add(new JudiciaryPersonErrorResponse
                    {
                        Message = bulkItemErrorMessage,
                        JudiciaryPersonRequest = item
                    });
                }
            }

            return Ok(bulkResponse);
        }

        [HttpPost("BulkJudiciaryLeavers")]
        [OpenApiOperation("BulkJudiciaryLeavers")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BulkJudiciaryLeaverResponse), (int) HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  BulkJudiciaryLeaversAsync(IEnumerable<JudiciaryLeaverRequest> request)
        {
            const string bulkItemErrorMessage = "Could not add or update external Judiciary user with Personal Code: {0}";
            var judiciaryLeaverRequests = request.ToList();
            _logger.LogInformation(
                "Starting BulkJudiciaryLeavers operation, processing {JudiciaryLeaversRequestsCount} items",
                judiciaryLeaverRequests.Count);

            var bulkResponse = new BulkJudiciaryLeaverResponse();

            foreach (var item in judiciaryLeaverRequests)
            {
                var validation = await new JudiciaryLeaverRequestValidation().ValidateAsync(item);
                if (!validation.IsValid)
                {
                    bulkResponse.ErroredRequests.Add(new JudiciaryLeaverErrorResponse
                    {
                        Message =
                            $"{string.Format(bulkItemErrorMessage, item.PersonalCode)} - {string.Join(", ", validation.Errors.Select(x => x.ErrorMessage))}",
                        JudiciaryLeaverRequest = item
                    });

                    continue;
                }

                try
                {
                    var query = new GetJudiciaryPersonByPersonalCodeQuery(item.PersonalCode);
                    var judiciaryPerson =
                        await _queryHandler.Handle<GetJudiciaryPersonByPersonalCodeQuery, JudiciaryPerson>(query);

                    if (judiciaryPerson != null)
                    {
                        await _commandHandler.Handle(
                            new UpdateJudiciaryLeaverByPersonalCodeCommand(item.PersonalCode, item.Leaver));
                    }
                    else
                    {
                        var message =
                            $"Unable to update the record in Judiciary Person with Personal Code - '{item.PersonalCode}'";
                        _logger.LogError(message);
                        bulkResponse.ErroredRequests.Add(new JudiciaryLeaverErrorResponse
                        {
                            Message = message,
                            JudiciaryLeaverRequest = item
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, bulkItemErrorMessage, item.PersonalCode);
                    bulkResponse.ErroredRequests.Add(new JudiciaryLeaverErrorResponse
                    {
                        Message = bulkItemErrorMessage,
                        JudiciaryLeaverRequest = item
                    });
                }
            }

            return Ok(bulkResponse);
        }

        /// <summary>
        /// Find persons with the email matching a search term.
        /// </summary>
        /// <param name="term">Partial string to match email with, case-insensitive.</param>
        /// <returns>Person list</returns>
        [HttpPost("search")]
        [OpenApiOperation("PostJudiciaryPersonBySearchTerm")]
        [ProducesResponseType(typeof(IList<PersonResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  PostJudiciaryPersonBySearchTerm(SearchTermRequest term)
        {
            if (_flagsService.GetFeatureFlag(nameof(FeatureFlags.EJudFeature)))
            {
                var query = new GetJudiciaryPersonBySearchTermQuery(term.Term);
                var personList =
                    await _queryHandler.Handle<GetJudiciaryPersonBySearchTermQuery, List<JudiciaryPerson>>(query);
                var mapper = new JudiciaryPersonToResponseMapper();
                var response = personList.Select(x => mapper.MapJudiciaryPersonToResponse(x)).OrderBy(o => o.Username)
                    .ToList();
                return Ok(response);
            }
            else
            {
                return Ok(new List<PersonResponse>());
            }
        }
    }
}