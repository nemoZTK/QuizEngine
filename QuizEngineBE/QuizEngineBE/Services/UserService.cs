using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using QuizEngineBE.DTO;
using QuizEngineBE.Models;

namespace QuizEngineBE.Services
{
    public class UserService(DbService dbServ,SecurityService sec)
    {
        private readonly DbService _dbServ=dbServ ;
        private readonly SecurityService _sec = sec;

        

        public async Task<List<User>> GetUsers()
        {
            return await _dbServ.GetAllUsersAsync();
        }


        public async Task<List<string>> GetUserNames()
        {
            return await _dbServ.GetAllUserNamesAsync();
        }


        public async Task<UserResponse> CreateNewUser(UserDTO user)
        {
            UserResponse response = new();

            if(!user.CheckFields) return response.MissingFields;
            
            user.Salt = _sec.GenerateSalt();
            user.Password = _sec.EncryptSHA256xBase64(user.Password + user.Salt);
            return await _dbServ.CreateUserAsync(user);

        }


        public async Task<UserResponse> IsValidRequest(LogOnRequest loginRequest)
        {
            UserResponse response= new();
            if (!loginRequest.CheckFields)return response.MissingFields;

            User user = await _dbServ.GetUserByNameAsync(loginRequest.Username);
            string password = _sec.EncryptSHA256xBase64(loginRequest.Password +user.Salt);
            
            if (password!= user.PasswordHash) return response.WrongFields;

            response.Success = true;
            response.Id = user.UserId;


            return response;
        }

        public  string GenerateJwtToken(string username)
        {
            return  _sec.GenerateJwtToken(username);
        }

        public async  Task<string> GetUsernameById(int id)
        {
            return await  _dbServ.GetUsernameById(id);
        }


        public (bool success, string message) IsUserAuthenticated(string user,string? token)
        {
            if (token == null) return (success: false, message: "almeno passalo un token dai");
            return _sec.ValidateJwtTokenForUser(user,token);
            
        }

    }
}
