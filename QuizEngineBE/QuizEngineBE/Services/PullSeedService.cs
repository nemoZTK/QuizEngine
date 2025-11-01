using QuizEngineBE.DTO.PullSeedSpace;
using QuizEngineBE.Interfaces;

namespace QuizEngineBE.Services
{
    //TODO: da implementare
    public class PullSeedService(DbService db) : IPullSeedService
    {

        private readonly DbService _dbServ = db;
        public async Task<QuizSeedResponse> CreateQuizSeed(QuizSeedDTO quizSeedDTO)
        {
            QuizSeedResponse response = new();

            if (!quizSeedDTO.CheckFields) return response.WrongFields();

            response.Id = await _dbServ.CreateQuizSeedAsync(quizSeedDTO);
            if (response.Id != null) response.Success = true;

            return response;
        }

        public async Task<PullResponse> DeletePull(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<QuizSeedResponse> DeleteQuizSeed(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PullDTO> GeneratePullFromQuizSeed()
        {
            throw new NotImplementedException();
        }

        public async Task<QuizSeedResponse> GetQuizSeedById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<QuizSeedResponse> GetQuizSeedsByQuizId(int quizId)
        {
            throw new NotImplementedException();
        }

        public Task<QuizSeedResponse> UpdateQuizSeed(QuizSeedDTO quizSeedDTO)
        {
            throw new NotImplementedException();
        }
    }
}
