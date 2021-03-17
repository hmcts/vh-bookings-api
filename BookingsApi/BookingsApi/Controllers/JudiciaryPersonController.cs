using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
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
            var bulkResponse = new BulkJudiciaryPersonResponse();
            
            foreach (var item in request)
            {
                var validation = await new JudiciaryPersonRequestValidation().ValidateAsync(item);
                if (!validation.IsValid)
                {
                    bulkResponse.ErroredRequests.Add(new JudiciaryPersonErrorResponse
                    {
                        Message = string.Join(", ", validation.Errors.Select(x => x.ErrorMessage)), 
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
                            item.Fullname, item.PostNominals, item.Email));
                    }
                    else
                    {
                        await _commandHandler.Handle(new UpdateJudiciaryPersonByExternalRefIdCommand(item.Id, item.PersonalCode, 
                            item.Title, item.KnownAs, item.Surname, item.Fullname, item.PostNominals, item.Email));
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Could not add or update external Judiciary user with External Id: {item.Id}";
                    _logger.LogError(ex, errorMessage);
                    bulkResponse.ErroredRequests.Add(new JudiciaryPersonErrorResponse
                    {
                        Message = errorMessage, JudiciaryPersonRequest = item
                    });
                }
            }
            
            return Ok(bulkResponse);
        }
    }
}