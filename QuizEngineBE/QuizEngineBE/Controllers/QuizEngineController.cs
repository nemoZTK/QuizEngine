using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using QuizEngineBE.DTO;
using QuizEngineBE.DTO.PullSeedSpace;
using QuizEngineBE.DTO.QuestionSpace;
using QuizEngineBE.DTO.QuizSpace;
using QuizEngineBE.DTO.UserSpace;
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


        protected string? GetToken() => Request.Headers.Authorization.FirstOrDefault();


        //[HttpGet("usernames")]
        //public async Task<IActionResult> GetAllUserNames()
        //{

        //    List<string> usernames = await _engine.GetUsernames();

        //    Log.Information(usernames.ToString());

        //    return Ok(usernames);

        //}


        //================================== RAGGIUNGIBILI SENZA TOKEN ====================================================

        
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

        [HttpGet("getQuizById")]
        public async Task<IActionResult> GetQuizById([FromQuery] int id, int? userId)
        {
            string? token = GetToken();
            QuizResponse response = await _engine.GetQuizById(id, userId, token);
            return ReturnWithAuthHeader(response, token);
        }





        //==================================== TOKEN NECESSARIO =======================================================

        [HttpPost("quiz")]
        public async Task<IActionResult> CreateQuiz([FromBody] QuizDTO quiz)
        {
            string? token = GetToken();
            QuizResponse response = await _engine.CreateQuiz(quiz, token);

            return ReturnWithAuthHeader(response, token);
        }
        [HttpGet("quizSeed")]
        public async Task<IActionResult> GetQuizseedsByQuizId([FromQuery]int quizId,[FromQuery] int? userId)
        {
            string? token = GetToken();
            QuizSeedResponse response = await _engine.GetQuizSeedsByQuizId(quizId, userId, token);

            return ReturnWithAuthHeader(response, token);
        }
        [HttpPost("addToQuiz")]
        public async Task<IActionResult> AddToQuiz([FromBody] QuestionsDTO questions)
        {
            string? token = GetToken(); 
            QuestionsResponse response = await _engine.AddQuestionsToQuiz(questions, token);
            
            return ReturnWithAuthHeader(response, token);
        }
        [HttpPost("quizSeed")]
        public async Task<IActionResult> AddQuizSeed([FromBody] QuizSeedDTO quizSeed)
        {
            string? token = GetToken();

            QuizSeedResponse response = await _engine.CreateQuizSeed(quizSeed, token);

            return ReturnWithAuthHeader(response, token);
        
        }


    }
}
