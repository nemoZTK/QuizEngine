using Azure;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using QuizEngineBE.DTO;
using QuizEngineBE.Models;
using System.Threading.Tasks;

namespace QuizEngineBE.Services
{
    public class QuizEngineService(UserService userServ,QuizService quizServ)
    {
        private readonly UserService _userServ = userServ;
        private readonly QuizService _quizServ = quizServ;

        public async Task<UserResponse> DoLogin(LogOnRequest loginRequest)
        {
            UserResponse response = await _userServ.IsValidRequest(loginRequest);

            if (response.Success == true)
            {
            response.Token = _userServ.GenerateJwtToken(loginRequest.Username);

            }

           
            return response;


        }

        public async Task<List<string>> GetUsernames()
        {
            return await _userServ.GetUserNames(); 
        }

        public async Task<UserResponse> RegisterUser(UserDTO user)
        {
            UserResponse response= await _userServ.CreateNewUser(user);
            if (response.Success==true)
            {
            response.Token= _userServ.GenerateJwtToken(user.Username);
            }
           
            return response;

        }

        public async Task<QuizResponse>GetQuizById(int id,int?userId, string? token)
        {
            QuizResponse response = new();
            response.Success = true;
            if(userId != null)
            (response.Success, response.Message) = await Authenticate(userId??0, token);
            if (response.Success != true) return response;


            return await _quizServ.GetQuizById(id, userId);
        }


        public async Task<QuizResponse> CreateQuiz(QuizDTO quiz,string? token)
        {
           
            QuizResponse response = new();
            (response.Success, response.Message) =await Authenticate(quiz.UserId, token);


            if (response.Success != true) return response;

            return await _quizServ.CreateQuiz(quiz);

        
        }

        public async Task<QuestionsResponse> AddQuestionsToQuiz(QuestionsDTO domande,string? token)
        {
            QuestionsResponse response = new();

            (response.Success, response.Message) =await Authenticate(domande.UserId, token);
            if (response.Success != true) return response;


            return await _quizServ.AddQuestionsToQuiz(domande);

        }


        private async Task<(bool success, string message)> Authenticate(int id, string? token)
        {
            string username = await _userServ.GetUsernameById(id);

            return  _userServ.IsUserAuthenticated(username, token);
        }


    }

}
