using QuizEngineBE.DTO;
using QuizEngineBE.DTO.ScoreboardSpace;
using QuizEngineBE.Interfaces;

namespace QuizEngineBE.Services
{
    //TODO: da implementare
    public class ScoreboardService : IScoreboardService
    {
        public Task<ScoreboardResponse> GetScoreboardByQuizSeedId(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateScoreboard<T>(ScoreboardDTO score) where T : ResponseBase<T>
        {
            throw new NotImplementedException();
        }
    }
}
