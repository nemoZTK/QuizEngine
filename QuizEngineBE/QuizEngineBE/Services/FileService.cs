using QuizEngineBE.DTO.QuizSpace;
using QuizEngineBE.Interfaces;

namespace QuizEngineBE.Services
{
    //TODO: da implementare
    public class FileService : IFileService
    {
        public (QuizDTO quiz, string? message, bool success) TryParse(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
