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


        protected IActionResult ReturnWithAuthHeader<T>(T response, string? token) where T : ResponseBase<T>
        {
            if (!string.IsNullOrWhiteSpace(token))
                Response.Headers.Append("Authorization", $"Bearer {token}");

            return response.Success ? Ok(response) : BadRequest(response);
        }


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

            return ReturnWithAuthHeader(response, response.Token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> DoLogin([FromBody]LogOnRequest loginRequest)
        {
   
            UserResponse response = await _engine.DoLogin(loginRequest);

            return ReturnWithAuthHeader(response, response.Token);
        }


        [HttpPost("quiz")]
        public async Task<IActionResult> CreateQuiz([FromBody] QuizDTO quiz)
        {
            string? token = Request.Headers["Authorization"].FirstOrDefault();
            QuizResponse response = await _engine.CreateQuiz(quiz, token);

            return ReturnWithAuthHeader(response, token);
        }


        [HttpPost("addToQuiz")]
        public async Task<IActionResult> AddToQuiz([FromBody] QuestionsDTO questions)
        {
            string? token = Request.Headers["Authorization"].FirstOrDefault();
            QuestionsResponse response = await _engine.AddQuestionsToQuiz(questions, token);
            
            return ReturnWithAuthHeader(response, token);
        }

        [HttpGet("getQuizById")]
        public async Task<IActionResult> GetQuizById([FromQuery] int id, int? userId )
        {
            string? token = Request.Headers["Authorization"].FirstOrDefault();
            QuizResponse response = await _engine.GetQuizById(id,userId,token);
            return ReturnWithAuthHeader(response, token);
        }

    }
}
