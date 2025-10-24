using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data.Common;
using QuizEngineBE.Models;
using QuizEngineBE.DTO;
namespace QuizEngineBE.Services
{
    public class DbService(QuizDbContext db, SecurityService sec) : DbBaseService<QuizDbContext>(db)
    {

        private readonly SecurityService _sec = sec;

        //============================== VARIE GET ==================


        public async Task<List<Quiz>> GetAllPublicQuizzesAsync()
        {
            return await SafeQueryAsync(async () =>
                await _db.Quizzes
                              .AsNoTracking()
                              .Where(q => q.Pubblico) 
                              .ToListAsync()
            ) ?? new List<Quiz>();
        }





        public async Task<UserResponse> CreateUserAsync(UserDTO utente)
        {
            var response = new UserResponse();

            var success = await SafeExecuteAsync(async () =>
            {
                var user = new User
                {
                    NomeUtente = utente.nomeUtente,
                    Email = utente.email,
                    PasswordHash = _sec.Encrypt(utente.password)
                };

                await _db.Users.AddAsync(user);
                await SaveChangesAsync(); 
                response.Id = user.UserId;
            });

            response.succes = success;
            response.message = success ? "Utente creato con successo" : "Errore nella creazione dell'utente";

            return response;
        }


        // Restituisce tutti gli utenti
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await SafeQueryAsync(async () =>
                await _db.Users.AsNoTracking().ToListAsync()
            ) ?? new List<User>();
        }


        public async Task<List<string>> GetAllUsersNamesAsync()
        {
            return await SafeQueryAsync(async () =>
                await _db.Users.AsNoTracking()
                 .Select(u => u.NomeUtente).ToListAsync()
            ) ?? new List<string>();

        }






    }
}
