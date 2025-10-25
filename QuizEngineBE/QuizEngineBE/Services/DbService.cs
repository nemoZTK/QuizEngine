using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data.Common;
using QuizEngineBE.Models;
using QuizEngineBE.DTO;

namespace QuizEngineBE.Services
{
    public class DbService(QuizDbContext db) : DbBaseService<QuizDbContext>(db)
    {

        public async Task<List<Quiz>> GetAllPublicQuizzesAsync(CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Quizzes
                          .AsNoTracking()
                          .Where(q => q.Pubblico)
                          .ToListAsync(token)
            , ct) ?? [];
        }

        public async Task<UserResponse> CreateUserAsync(UserDTO utente, CancellationToken ct = default)
        {
            var response = new UserResponse();

            response.Success = await SafeExecuteAsync(async token =>
            {
                var user = new User
                {
                    NomeUtente = utente.Username,
                    Email = utente.Email,
                    PasswordHash = utente.Password,
                    Salt = (utente.Salt!=null)?utente.Salt : "sale&pepe" ,
                    Ruolo = utente.Ruolo ?? "1"
                };

                await _db.Users.AddAsync(user, token);
                await SaveChangesAsync(token);
                response.Id = user.UserId;
            }, ct);

            response.Message = response.Success ? "Utente creato con successo" : "Errore nella creazione dell'utente";

            return response;
        }

        public async Task<List<User>> GetAllUsersAsync(CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Users.AsNoTracking().ToListAsync(token)
            , ct) ?? [];
        }

        public async Task<List<string>> GetAllUserNamesAsync(CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Users.AsNoTracking()
                               .Select(u => u.NomeUtente)
                               .ToListAsync(token)
            , ct) ?? [];
        }

        internal async Task<User> GetUserByNameAsync(string name, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Users.AsNoTracking()
                               .Where(u => u.NomeUtente == name)
                               .FirstOrDefaultAsync(token)
            , ct) ?? new User();
        }

        internal async Task<bool> UserExistByName(string name, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Users.AsNoTracking()
                               .AnyAsync(u => u.NomeUtente == name, token)
            , ct);
        }



        internal async Task<QuizResponse> CreateQuizAsync(QuizDTO request, CancellationToken ct = default)
        {
            var response = new QuizResponse();

            response.Success = await SafeExecuteAsync(async token =>
            {
                var quiz = new Quiz
                {
                    Nome = request.Name,
                    ValoriDifficolta = request.DifficultValues,
                    UserId = request.UserId,
                    Pubblico = request.Pubblico
                };

                await _db.Quizzes.AddAsync(quiz, token);
                await SaveChangesAsync(token);
                response.Id = quiz.QuizId;
            }, ct);

            response.Message = response.Success ? "Quiz creato con successo" : "Errore nella creazione del quiz";
            
            return response;

        }

        internal async Task<string> GetUsernameById(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Users
                         .AsNoTracking()
                         .Where(u => u.UserId == id)
                         .Select(u => u.NomeUtente)
                         .FirstOrDefaultAsync(token) 
            , ct) ?? string.Empty;
        }

    }
}
