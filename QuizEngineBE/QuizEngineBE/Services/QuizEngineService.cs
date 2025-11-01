using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using QuizEngineBE.DTO.PullSeedSpace;
using QuizEngineBE.DTO.QuestionSpace;
using QuizEngineBE.DTO.QuizSpace;
using QuizEngineBE.DTO.ScoreboardSpace;
using QuizEngineBE.DTO.UserSpace;
using QuizEngineBE.Interfaces;
using QuizEngineBE.Models;
using System.Threading.Tasks;

namespace QuizEngineBE.Services
{
    public class QuizEngineService(UserService userServ, QuizService quizServ, PullSeedService pullSeedServ) : IQuizEngineService
    {
        private readonly UserService     _userServ     = userServ;
        private readonly QuizService     _quizServ     = quizServ;
        private readonly PullSeedService _pullSeedServ = pullSeedServ;


        //=============================================== METODI PRIVATI ======================================================================
        /// <summary>
        /// metodo privato che autorizza un utente dato il suo id ed eventualmente un token,
        /// <br/>(ovviamente il token per fare il confronto andrebbe passato, ma se non si riesce a recuperarlo si può non passare
        /// </summary>
        /// <returns></returns>
        private async Task<(bool success, string message)> AuthorizeUser(int id, string? token)
        {
            string username = await _userServ.GetUsernameById(id);
            return  _userServ.IsUserAuthorized(username, token);
        }
        


        //============================================= METODI PUBBLICI ==============================================================================


        //======================= LATO UTENTE ==================================================
        public async Task<UserResponse> DoLogin(LogOnRequest loginRequest)
        {
            UserResponse response = await _userServ.TryToDoLogin(loginRequest);

            if (response.Success)
                response.Token = _userServ.GenerateJwtToken(loginRequest.Username);

            


            return response;


        }

        //public async Task<List<string>> GetUsernames()
        //{
        //    return await _userServ.GetUserNames(); 
        //}

        public async Task<UserResponse> RegisterUser(UserDTO user)
        {
            UserResponse response = await _userServ.CreateNewUser(user);
            
            if (response.Success)
                response.Token = _userServ.GenerateJwtToken(user.Username);
            

            return response;

        }

        public Task<UserResponse> UpdateUserInfo(UserDTO user, string? token)
        {
            throw new NotImplementedException();
        }

        public Task<UserResponse> DeleteUser(string id, string? token)
        {
            throw new NotImplementedException();
        }


        //============================ LATO QUIZ ==================================
        public async Task<QuizResponse> GetQuizById(int id, int? userId, string? token)
        {
            QuizResponse response = new();
            
            if (userId != null)
            {
                (response.Success, response.Message) = await AuthorizeUser(userId ?? 0, token);
                if (!response.Success) return response;
            }


            return await _quizServ.GetQuizById(id, userId);
        }


        public async Task<QuizResponse> CreateQuiz(QuizDTO quiz, string? token)
        {

            QuizResponse response = new();
            
            (response.Success, response.Message) = await AuthorizeUser(quiz.UserId, token);


            if (!response.Success) return response;

            return await _quizServ.CreateQuiz(quiz);


        }



        public Task<QuizResponse> UpdateQuiz(QuizDTO quiz, string? token)
        {
            throw new NotImplementedException();
        }

        public Task<QuizResponse> DeleteQuiz(int id)
        {
            throw new NotImplementedException();
        }

        //================================= LATO DOMANDE ===============================================
        public async Task<QuestionsResponse> AddQuestionsToQuiz(QuestionsDTO domande, string? token)
        {
            QuestionsResponse response = new();

            (response.Success, response.Message) = await AuthorizeUser(domande.UserId, token);
            if (!response.Success) return response;


            return await _quizServ.AddQuestionsToQuiz(domande);

        }

        public Task<QuestionsResponse> UpdateQuestions(QuestionsDTO questions, string? token)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionsResponse> DeleteQuestions(QuestionsDTO questions, string? token)
        {
            throw new NotImplementedException();
        }

        //================================= LATO QUIZSEED =================================================

        public async Task<QuizSeedResponse> CreateQuizSeed(QuizSeedDTO quizSeed, string? token)
        {
            QuizSeedResponse response = new();

            response = await _quizServ.CanSeeQuiz(response, quizSeed.QuizId, quizSeed.UserId);

            (response.Success,response.Message) = await AuthorizeUser(quizSeed.UserId, token);
            if (!response.Success) return response;

            if(await _quizServ.GetQuestionNumber(quizSeed.QuizId) < quizSeed.QuestionNumner)
                return response.WrongFields("numero domande maggiore del totale");

            return await _pullSeedServ.CreateQuizSeed(quizSeed);



        }
        
        public async Task<QuizSeedResponse> GetQuizSeedsByQuizId(int quizId, int? userId,string? token)
        {
            QuizSeedResponse response = new();

            response = await _quizServ.CanSeeQuiz(response, quizId, userId);
            if(userId != null)
            {
                (response.Success, response.Message) = await AuthorizeUser(userId??0, token);
                if (!response.Success) return response;
            }

            return await _pullSeedServ.GetQuizSeedsByQuizId(quizId, userId);


        }


        //=================================== LATO SCOREBOARD ===============================================

        public Task<ScoreboardResponse> GetScoreboardByQuizSeedIdAndUserId(int id, int? userId, string? token)
        {
            throw new NotImplementedException();
        }

    }


}