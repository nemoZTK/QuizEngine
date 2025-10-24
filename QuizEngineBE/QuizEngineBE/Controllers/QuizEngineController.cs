using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizEngineBE.DTO;
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

        [HttpPost("user")]
        public async Task<IActionResult> AddNewUser(UserDTO user)
        {
            UserResponse response = await _engine.CreateNewUser(user);
            Log.Information($"New user request : {response}");

            return Ok(response);
        }



    }
}
