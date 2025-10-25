using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
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

            List<string> usernames = await _engine.GetUsernames();

            Log.Information(usernames.ToString());

            return Ok(usernames);
        
        }

        [HttpPost("user")]
        public async Task<IActionResult> AddNewUser(UserDTO user)
        {
            UserResponse response = await _engine.RegisterUser(user);
            Log.Information($"New user request : {response}");
            if (response.Token != null)
            {
                Response.Headers.Append("Authorization", $"Bearer {response.Token}");
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("login")]

        public async Task<IActionResult> DoLogin([FromBody]LogOnRequest loginRequest)
        {
   
            UserResponse response = await _engine.DoLogin(loginRequest);
            if (response.Token != null)
            {
                Response.Headers.Append("Authorization", $"Bearer {response.Token}");
                return Ok(response);
            }

            return BadRequest(response);

        }


        [HttpPost("quiz")]
        public async Task<IActionResult> CreateQuiz([FromBody] QuizDTO quiz)
        {
            string? token = Request.Headers["Authorization"].FirstOrDefault();
            QuizResponse response = await _engine.CreateQuiz(quiz, token);

            return Ok(response);
        }



    }
}
