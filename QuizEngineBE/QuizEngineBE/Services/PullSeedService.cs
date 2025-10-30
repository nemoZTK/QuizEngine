using QuizEngineBE.DTO.PullSeedSpace;
using QuizEngineBE.Interfaces;

namespace QuizEngineBE.Services
{
    //TODO: da implementare
    public class PullSeedService : IPullSeedService
    {
        public Task<QuizSeedResponse> CreateQuizSeed(QuizSeedDTO quizSeedDTO)
        {
            throw new NotImplementedException();
        }

        public Task<PullResponse> DeletePull(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuizSeedResponse> DeleteQuizSeed(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PullDTO> GeneratePullFromQuizSeed()
        {
            throw new NotImplementedException();
        }

        public Task<QuizSeedResponse> GetQuizSeedById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<QuizSeedResponse> GetQuizSeedsByQuizId(int quizId)
        {
            throw new NotImplementedException();
        }

        public Task<QuizSeedResponse> UpdateQuizSeed(QuizSeedDTO quizSeedDTO)
        {
            throw new NotImplementedException();
        }
    }
}
