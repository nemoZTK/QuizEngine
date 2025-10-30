using QuizEngineBE.DTO.PullSeedSpace;

namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// service che si occupa della randomizzazione delle domande
    /// </summary>
    public interface IQuizShaflerService
    {

        Task<PullDTO> ShaffleQuestions();
    }
}
