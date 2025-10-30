using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using QuizEngineBE.DTO.UserSpace;
using QuizEngineBE.Interfaces;
using QuizEngineBE.Models;

namespace QuizEngineBE.Services
{
    public class UserService(DbService dbServ,SecurityService sec) : IUserService
    {
        private readonly DbService _dbServ=dbServ ;
        private readonly SecurityService _sec = sec;

        

        //public async Task<List<UserSpace>> GetUsers()
        //{
        //    return await _dbServ.GetAllUsersAsync();
        //}


        //public async Task<List<string>> GetUserNames()
        //{
        //    return await _dbServ.GetAllUserNamesAsync();
        //}


        public async Task<UserResponse> CreateNewUser(UserDTO user)
        {
            UserResponse response = new();

            if(!user.CheckFields) return response.MissingFields();
            if(await _dbServ.UserExistByNameAsync(user.Username))return response.ExistingUser;
            
            user.Salt = _sec.GenerateSalt();
            user.Password = _sec.EncryptSHA256xBase64(user.Password + user.Salt);
            return await _dbServ.CreateUserAsync(user);

        }


        public async Task<UserResponse> TryToDoLogin(LogOnRequest loginRequest)
        {
            UserResponse response= new();
            if (!loginRequest.CheckFields)return response.MissingFields();

            UserDTO user = await _dbServ.GetUserByNameAsync(loginRequest.Username);
            string password = _sec.EncryptSHA256xBase64(loginRequest.Password +user.Salt);
            
            if (password!= user.Password) return response.WrongFields;

            response.Success = true;
            response.Id = user.Id;


            return response;
        }

        public  string GenerateJwtToken(string username) =>  _sec.GenerateJwtToken(username);
        

        public async  Task<string> GetUsernameById(int id) => await  _dbServ.GetUsernameByIdAsync(id);
        


        public (bool success, string message) IsUserAuthorized(string user,string? token)
        {
            if (token == null) return (success: false, message: "almeno passalo un token dai");
            return _sec.ValidateJwtTokenForUser(user,token);
            
        }

    }
}
