using Microsoft.IdentityModel.Tokens;
using QuizEngineBE.DTO;
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


        public async Task<UserResponse> CreateNewUser(UserDTO user)
        {
            UserResponse response = new UserResponse();

            if (user.nomeUtente.IsNullOrEmpty()||
                user.password.IsNullOrEmpty()||
                user.email.IsNullOrEmpty())
            {
                response.succes = false;
                response.message = "campi mancanti";
                return response;

            }
            return await _db.CreateUserAsync(user);

        }



    }

}
