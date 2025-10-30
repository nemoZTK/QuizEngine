using QuizEngineBE.DTO;
using QuizEngineBE.DTO.ScoreboardSpace;

namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// service che si occupa dell' ottenimento ed inserimento delle socreboards 
    /// <br/> usa DbService
    /// </summary>
    public interface IScoreboardService
    {

        Task<ScoreboardResponse> GetScoreboardByQuizSeedId(int id,int userId);



        Task<T> UpdateScoreboard<T>(ScoreboardDTO score) where T : ResponseBase<T>;






    }
}
