using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data.Common;
using QuizEngineBE.Models;
using QuizEngineBE.DTO;

namespace QuizEngineBE.Services
{
    public class DbService(QuizDbContext db) : DbBaseService<QuizDbContext>(db)
    {



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
                    Salt = utente.Salt
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

        internal async Task<UserDTO> GetUserByNameAsync(string name, CancellationToken ct = default)
        {
            var user = await SafeQueryAsync(async token =>
                await _db.Users.AsNoTracking()
                               .Where(u => u.NomeUtente == name)
                               .FirstOrDefaultAsync(token)
            , ct);

            if (user == null)
                return new UserDTO(); // ritorna DTO vuoto se non trovato

            return new UserDTO
            {
                Id = user.UserId,
                Username = user.NomeUtente,
                Password = user.PasswordHash,
                Email = user.Email??"mail mancante",
                Salt = user.Salt,
                Ruolo = user.Ruolo
            };
        }

        internal async Task<bool> UserExistByName(string name, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Users.AsNoTracking()
                               .AnyAsync(u => u.NomeUtente == name, token)
            , ct);
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

        public async Task<List<Quiz>> GetAllPublicQuizzesAsync(CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Quizzes
                          .AsNoTracking()
                          .Where(q => q.Pubblico)
                          .ToListAsync(token)
            , ct) ?? [];
        }

        internal async Task<int?> CreateQuizAsync(QuizDTO request, CancellationToken ct = default)
        {
            int? quizId = null;

            bool success = await SafeExecuteAsync(async token =>
            {
                var quiz = new Quiz
                {
                    Nome = request.Name,
                    ValoriDifficolta = request.DifficultValues,
                    UserId = request.UserId,
                    Pubblico = request.Public
                };

                await _db.Quizzes.AddAsync(quiz, token);
                await SaveChangesAsync(token);

                quizId = quiz.QuizId;
            }, ct);

            return success ? quizId : null;
        }




        internal async Task<QuizDTO> GetQuizById(int id, CancellationToken ct = default)
        {
            var quiz = await SafeQueryAsync(
                async token => await _db.Quizzes
                    .AsNoTracking()
                    .Where(u => u.QuizId == id)
                    .FirstOrDefaultAsync(token),
                ct
            );

            // Ritorna un DTO vuoto se non è stato trovato alcun quiz
            if (quiz == null)
                return new QuizDTO();

            return new QuizDTO
            {
                Id = id,
                DifficultValues = quiz.ValoriDifficolta ?? "",
                UserId = quiz.UserId,
                Name = quiz.Nome,
                Public = quiz.Pubblico
            };
        }

        internal async Task<QuestionsDTO> GetQuestionsByQuizId(int quizId, CancellationToken ct = default)
        {
            var questionsDto = new QuestionsDTO
            {
                QuizId = quizId,
                Questions = new List<Question>()
            };

            // Recupera tutte le domande associate al quiz
            var domande = await SafeQueryAsync(async token =>
                await _db.Domanda
                         .AsNoTracking()
                         .Where(d => d.QuizId == quizId)
                         .ToListAsync(token)
            , ct) ?? new List<Domanda>();

            foreach (var d in domande)
            {
                var question = new Question
                {
                    Text = d.Testo,
                    RightAnswersString = d.RisposteGiuste,
                    WrongAnswersString = d.RisposteSbagliate,
                    Time = d.TempoRisposta,
                    Difficulty = d.Difficolta,
                    Sequence = d.Sequenza,
                    SequenceNumber = d.NumeroSequenza,
                    Variant = d.Variante,
                    RightAnswers = !string.IsNullOrWhiteSpace(d.RisposteGiuste)
                                   ? new List<string>(d.RisposteGiuste.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                   : new List<string>(),
                    WrongAnswers = !string.IsNullOrWhiteSpace(d.RisposteSbagliate)
                                   ? new List<string>(d.RisposteSbagliate.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                   : new List<string>()
                };

                questionsDto.Questions.Add(question);
            }

            return questionsDto;
        }

        internal async Task<List<int>> AddQuestionsToQuiz(QuestionsDTO request, CancellationToken ct = default)
        {
            var createdQuestions = new List<Domanda>();

            bool success = await SafeExecuteAsync(async token =>
            {
                foreach (var d in request.Questions)
                {
                    if (string.IsNullOrWhiteSpace(d.WrongAnswersString) ||
                        string.IsNullOrWhiteSpace(d.RightAnswersString))
                    {
                        throw new InvalidOperationException(
                            $"Wrong or Right answers null or empty ==>\n W --> [{d.WrongAnswersString}] \n R ->[{d.RightAnswersString}]");
                    }

                    var domanda = new Domanda
                    {
                        Testo = d.Text,
                        Difficolta = d.Difficulty,
                        QuizId = request.QuizId,
                        RisposteGiuste = d.RightAnswersString,
                        RisposteSbagliate = d.WrongAnswersString,
                        TempoRisposta = d.Time,
                        Sequenza = d.Sequence,
                        NumeroSequenza = d.SequenceNumber,
                        Variante = d.Variant
                    };

                    await _db.Domanda.AddAsync(domanda, token);
                    createdQuestions.Add(domanda);
                }

                await SaveChangesAsync(token);
            }, ct);

            return success ? createdQuestions.Select(q => q.DomandaId).ToList() : [];
        }


        internal async Task<bool> QuizExistById(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Quizzes.AsNoTracking()
                                 .AnyAsync(q => q.QuizId == id, token)
            , ct);
        }
        internal async Task<int?> GetQuizUserIdById(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Quizzes.AsNoTracking()
                                 .Where(q => q.QuizId == id)
                                 .Select(q => (int?)q.UserId)
                                 .FirstOrDefaultAsync(token)
            , ct);
        }

        /// <summary>
        /// query fatta su misura per sapere se un quiz esiste o meno e se è pubblico. 
        /// <br/>cerca dentro quiz il quizId e se lo trova prende userId e il valore di pubblico
        /// <br/>li restituisce in un oggetto QuizPublicDTO
        /// </summary>
        /// <returns>un oggetto QuizPublicDTO o null  </returns>
        internal async Task<QuizPublicDTO?> GetQuizPublicDataById(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Quizzes.AsNoTracking()
                                 .Where(q => q.QuizId == id)
                                 .Select(q => new QuizPublicDTO(q.UserId, q.Pubblico))
                                 .FirstOrDefaultAsync(token)
            , ct);
        }



        internal async Task<bool?> GetQuizPublicStatusById(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Quizzes.AsNoTracking()
                                 .Where(q => q.QuizId == id)
                                 .Select(q => (bool?)q.Pubblico)
                                 .FirstOrDefaultAsync(token)
            , ct);
        }



        internal async Task<bool> QuizHaveQuestions(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Domanda.AsNoTracking()
                                 .AnyAsync(q => q.QuizId == id, token)
            , ct);
        }

    }
}
