using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data.Common;
using QuizEngineBE.Models;
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




 
        public async Task<bool> CreateUserAsync(string nomeUtente, string email, string password)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = new User
                {
                    NomeUtente = nomeUtente,
                    Email = email,
                    PasswordHash = _sec.Encrypt(password)
                };

                await _db.Users.AddAsync(user);
            });
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
