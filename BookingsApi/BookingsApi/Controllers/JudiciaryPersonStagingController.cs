using System.Net;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace BookingsApi.Controllers
{
    public class JudiciaryPersonStagingController : Controller
    {
        private readonly ICommandHandler _commandHandler;
        
        public JudiciaryPersonStagingController(ICommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }
        
        [HttpGet("RemoveAllJudiciaryPersonsStaging")]
        [OpenApiOperation("RemoveAllJudiciaryPersonsStaging")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> RemoveAllJudiciaryPersonsStaging()
        {
            await _commandHandler.Handle(new RemoveAllJudiciaryPersonStagingCommand());

            return Ok();
        }

    }
}