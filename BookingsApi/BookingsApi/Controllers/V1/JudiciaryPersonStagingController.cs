using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Commands.V1;
using BookingsApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("judiciarypersonstaging")]
    [ApiController]
    [ApiVersion("1.0")]
    public class JudiciaryPersonStagingController : Controller
    {
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<JudiciaryPersonStagingController> _logger;

        public JudiciaryPersonStagingController(ICommandHandler commandHandler, ILogger<JudiciaryPersonStagingController> logger)
        {
            _commandHandler = commandHandler;
            _logger = logger;
        }
        
        [HttpDelete("RemoveAllJudiciaryPersonsStaging")]
        [OpenApiOperation("RemoveAllJudiciaryPersonsStaging")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  RemoveAllJudiciaryPersonsStagingAsync()
        {
            await _commandHandler.Handle(new RemoveAllJudiciaryPersonStagingCommand());

            return Ok();
        }

        [HttpPost("BulkJudiciaryPersonsStaging")]
        [OpenApiOperation("BulkJudiciaryPersonsStaging")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  BulkJudiciaryPersonsStagingAsync(IEnumerable<JudiciaryPersonStagingRequest> request)
        {
            var judiciaryPersonStagingRequests = request.ToList();
            
            foreach (var item in judiciaryPersonStagingRequests)
            {
                try
                {
                    await _commandHandler.Handle(AddJudiciaryPersonStagingCommandMapper.Map(item));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BookingsApi - BulkJudiciaryPersonsStagingAsync | error updating user");
                }
                
            }
            
            return Ok();
        }
    }
}