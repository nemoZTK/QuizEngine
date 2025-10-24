using QuizEngineBE.Models;
using System.Threading.Tasks;

namespace QuizEngineBE.Services
{
    public class QuizEngineService(DbService db)
    {

        private readonly DbService _db = db;

        public async Task<List<User>> GetUsers()
        {
            return await  _db.GetAllUsersAsync();
        }


        public async Task<List<string>> GetUsersNames()
        {
            return await _db.GetAllUsersNamesAsync();
        }


      



    }
}
