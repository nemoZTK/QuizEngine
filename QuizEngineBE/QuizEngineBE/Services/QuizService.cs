using Microsoft.Identity.Client;
using QuizEngineBE.DTO;

namespace QuizEngineBE.Services
{
    public class QuizService(DbService db)
    {

        private readonly DbService _dbServ= db;


        public string ParseDictionary(Dictionary<string, int> dict) => string.Join(";", dict.Select(kvp => $"<{kvp.Key}<{kvp.Value}>>"));



        public async Task<QuizResponse> CreateQuiz(QuizDTO request)
        {
           QuizResponse response = new();
            if (!request.CheckFields) return response.MissingFields;

            if (request.Difficulties != null) request.DifficultValues = ParseDictionary(request.Difficulties);
            
            return await _dbServ.CreateQuizAsync(request);

        }


    }
}
