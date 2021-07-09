using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Mappings;
using BookingsApi.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("judiciaryperson")]
    [ApiController]
    public class JudiciaryPersonController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<JudiciaryPersonController> _logger;
        
        public JudiciaryPersonController(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<JudiciaryPersonController> logger)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _logger = logger;
        }
        
        [HttpPost]
        [OpenApiOperation("BulkJudiciaryPersons")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BulkJudiciaryPersonResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> BulkJudiciaryPersonsAsync(IEnumerable<JudiciaryPersonRequest> request)
        {
            const string bulkItemErrorMessage = "Could not add or update external Judiciary user with External Id: {0}";
            var judiciaryPersonRequests = request.ToList();
            _logger.LogInformation("Starting BulkJudiciaryPersons operation, processing {JudiciaryPersonRequestsCount} items", judiciaryPersonRequests.Count);
            
            var bulkResponse = new BulkJudiciaryPersonResponse();
            
            foreach (var item in judiciaryPersonRequests)
            {
                var validation = await new JudiciaryPersonRequestValidation().ValidateAsync(item);
                if (!validation.IsValid)
                {
                    bulkResponse.ErroredRequests.Add(new JudiciaryPersonErrorResponse
                    {
                        Message = $"{string.Format(bulkItemErrorMessage, item.Id)} - {string.Join(", ", validation.Errors.Select(x => x.ErrorMessage))}", 
                        JudiciaryPersonRequest = item
                    });
                    
                    continue;
                }
            
                try
                {
                    var query = new GetJudiciaryPersonByExternalRefIdQuery(item.Id);
                    var judiciaryPerson = await _queryHandler.Handle<GetJudiciaryPersonByExternalRefIdQuery, JudiciaryPerson>(query);

                    if (judiciaryPerson == null)
                    {
                        await _commandHandler.Handle(new AddJudiciaryPersonCommand(item.Id, item.PersonalCode, item.Title, item.KnownAs, item.Surname,
                            item.Fullname, item.PostNominals, item.Email, item.HasLeft));
                    }
                    else
                    {
                        await _commandHandler.Handle(new UpdateJudiciaryPersonByExternalRefIdCommand(item.Id, item.PersonalCode, 
                            item.Title, item.KnownAs, item.Surname, item.Fullname, item.PostNominals, item.Email, item.HasLeft));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, bulkItemErrorMessage, item.Id);
                    bulkResponse.ErroredRequests.Add(new JudiciaryPersonErrorResponse
                    {
                        Message = bulkItemErrorMessage, JudiciaryPersonRequest = item
                    });
                }
            }
            
            return Ok(bulkResponse);
        }

        [HttpPost]
        [OpenApiOperation("BulkJudiciaryLeavers")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BulkJudiciaryLeaverResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> BulkJudiciaryLeaversAsync(IEnumerable<JudiciaryLeaverRequest> request)
        {
            const string bulkItemErrorMessage = "Could not add or update external Judiciary user with External Id: {0}";
            var judiciaryLeaverRequests = request.ToList();
            _logger.LogInformation("Starting BulkJudiciaryLeavers operation, processing {JudiciaryLeaversRequestsCount} items", judiciaryLeaverRequests.Count);

            var bulkResponse = new BulkJudiciaryLeaverResponse();

            foreach (var item in judiciaryLeaverRequests)
            {
                var validation = await new JudiciaryLeaverRequestValidation().ValidateAsync(item);
                if (!validation.IsValid)
                {
                    bulkResponse.ErroredRequests.Add(new JudiciaryLeaverErrorResponse
                    {
                        Message = $"{string.Format(bulkItemErrorMessage, item.Id)} - {string.Join(", ", validation.Errors.Select(x => x.ErrorMessage))}",
                        JudiciaryLeaverRequest = item
                    });

                    continue;
                }

                try
                {
                    var query = new GetJudiciaryPersonByExternalRefIdQuery(item.Id);
                    var judiciaryPerson = await _queryHandler.Handle<GetJudiciaryPersonByExternalRefIdQuery, JudiciaryPerson>(query);

                    if (judiciaryPerson != null)
                    {
                        await _commandHandler.Handle(new UpdateJudiciaryLeaverByExternalRefIdCommand(item.Id, item.Leaver));
                    }
                    else
                    {
                        _logger.LogError($"Unable to update the record in Judiciary Person with ExternalRefId '{item.Id}'");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, bulkItemErrorMessage, item.Id);
                    bulkResponse.ErroredRequests.Add(new JudiciaryLeaverErrorResponse
                    {
                        Message = bulkItemErrorMessage, JudiciaryLeaverRequest = item
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
        [ProducesResponseType(typeof(IList<PersonResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostJudiciaryPersonBySearchTerm(SearchTermRequest term)
        {
            var query = new GetJudiciaryPersonBySearchTermQuery(term.Term);
            var personList = await _queryHandler.Handle<GetJudiciaryPersonBySearchTermQuery, List<JudiciaryPerson>>(query);
            var mapper = new JudiciaryPersonToResponseMapper();
            var response = personList.Select(x => mapper.MapJudiciaryPersonToResponse(x)).OrderBy(o => o.Username).ToList();
            return Ok(response);
        }
    }
}