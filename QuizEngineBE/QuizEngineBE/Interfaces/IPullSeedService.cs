using QuizEngineBE.DTO.PullSeedSpace;

namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// service che si occupa dell' ottenimento, creazione, modifica ed eliminazione  dei quizSeeds e dei relativi pull (quest'ultimi non modificabili)
    /// <br/> usa DbService 
    /// </summary>
    public interface IPullSeedService
    {
        /// <summary>
        /// ottiene una lista di quizSeed associati ad un quizId e ad un userId
        /// </summary>
        /// <returns>una QuizSeedResponse</returns>
        Task<QuizSeedResponse> GetQuizSeedsByQuizId(int quizId,int? userId);

        /// <summary>
        /// ottiene un quizSeed dato il suo id
        /// </summary>
        /// <returns></returns>
        Task<QuizSeedResponse> GetQuizSeedById(int id);
        /// <summary>
        /// crea un QuizSeed dato un QuizSeedDTO
        /// </summary>
        /// <param name="quizSeedDTO"></param>
        /// <returns>una QuizSeedResponse</returns>
        Task<QuizSeedResponse> CreateQuizSeed(QuizSeedDTO quizSeedDTO);

        
        Task<QuizSeedResponse> UpdateQuizSeed(QuizSeedDTO quizSeedDTO);


        Task<QuizSeedResponse> DeleteQuizSeed(int id);


        Task<PullResponse> DeletePull(int id);


        
    }
}
