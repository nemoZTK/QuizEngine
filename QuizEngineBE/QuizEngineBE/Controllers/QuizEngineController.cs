using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizEngineBE.Services;
using Serilog;
using System.Threading.Tasks;

namespace QuizEngineBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizEngineController(QuizEngineService engine) : ControllerBase
    {
        private readonly QuizEngineService _engine = engine;

        [HttpGet("usernames")]
        public async Task<IActionResult> GetAllUserNames()
        {

            List<string> usernames = await _engine.GetUsersNames();

            Log.Information(usernames.ToString());

            return Ok(usernames);
        
        }



    }
}
