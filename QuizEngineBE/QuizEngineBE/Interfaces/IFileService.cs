using QuizEngineBE.DTO.QuizSpace;

namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// service che si occupa del parsing dei file importati (contenenti i quiz)
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// prova a convertire un IFromFile in un QuizDTO
        /// </summary>
        /// <returns>un QuizDTO</returns>
        (QuizDTO quiz,string? message,bool success) TryParse(IFormFile file);




    }
}
