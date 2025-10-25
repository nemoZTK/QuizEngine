using Microsoft.AspNetCore.Identity.Data;
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

        public async Task<QuizResponse> CreateQuiz(QuizDTO quiz,string? token)
        {
           
            QuizResponse response = new();

            string username = await _userServ.GetUsernameById(quiz.UserId);
            
            (response.Success,response.Message) = _userServ.IsUserAuthenticated(username, token);
            
            if (response.Success != true) return response;

            return await _quizServ.CreateQuiz(quiz);

        
        }





    }

}
