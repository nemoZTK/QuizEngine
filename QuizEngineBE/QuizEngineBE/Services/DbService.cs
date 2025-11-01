using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data.Common;
using QuizEngineBE.Models;
using QuizEngineBE.DTO.QuestionSpace;
using QuizEngineBE.DTO.QuizSpace;
using QuizEngineBE.DTO.UserSpace;
using QuizEngineBE.Interfaces;
using QuizEngineBE.DTO.PullSeedSpace;
using QuizEngineBE.DTO.ScoreboardSpace;

namespace QuizEngineBE.Services
{

    public class DbService(QuizDbContext db) : DbBaseService<QuizDbContext>(db) , IDbService
    {




        //========================= LATO UTENTE =======================================
        //TODO: andrebbe conformato alle altre create (quindi tornare l'id, poi il dbservice non dovrebbe conoscere le response, solo i dto)
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
                    Salt = utente.Salt,
                    Ruolo = utente.Ruolo
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

        public async Task<UserDTO> GetUserByNameAsync(string name, CancellationToken ct = default)
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

        public async Task<bool> UserExistByNameAsync(string name, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Users.AsNoTracking()
                               .AnyAsync(u => u.NomeUtente == name, token)
            , ct);
        }


        public async Task<string> GetUsernameByIdAsync(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Users
                         .AsNoTracking()
                         .Where(u => u.UserId == id)
                         .Select(u => u.NomeUtente)
                         .FirstOrDefaultAsync(token)
            , ct) ?? string.Empty;
        }






        public Task<bool> DeleteUserByIdAsync(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserAsync(UserDTO utente, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        //========================= LATO QUIZ =======================================

        public async Task<List<QuizDTO>> GetAllPublicQuizzesAsync(CancellationToken ct = default)
        {
            var quizzes= await SafeQueryAsync(async token =>
                await _db.Quizzes
                          .AsNoTracking()
                          .Where(q => q.Pubblico)
                          .ToListAsync(token)
            , ct) ?? [];
            List<QuizDTO> quizDTOs = [];
            foreach(var quiz in quizzes)
            {
                quizDTOs.Add(new QuizDTO() 
                {
                    Id = quiz.QuizId,
                    Name = quiz.Nome,
                    DifficultValues = quiz.ValoriDifficolta,
                    Public = quiz.Pubblico,
                    UserId = quiz.UserId
                });
                
            }
            return quizDTOs;
        }

        public async Task<int?> CreateQuizAsync(QuizDTO request, CancellationToken ct = default)
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




        public async Task<QuizDTO> GetQuizByIdAsync(int id, CancellationToken ct = default)
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


        public async Task<bool> QuizExistByIdAsync(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Quizzes.AsNoTracking()
                                 .AnyAsync(q => q.QuizId == id, token)
            , ct);
        }
        public async Task<int?> GetQuizUserIdByIdAsync(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Quizzes.AsNoTracking()
                                 .Where(q => q.QuizId == id)
                                 .Select(q => (int?)q.UserId)
                                 .FirstOrDefaultAsync(token)
            , ct);
        }


        public async Task<QuizPublicDTO?> GetQuizPublicDataByIdAsync(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Quizzes.AsNoTracking()
                                 .Where(q => q.QuizId == id)
                                 .Select(q => new QuizPublicDTO(q.UserId, q.Pubblico))
                                 .FirstOrDefaultAsync(token)
            , ct);
        }

        public async Task<bool> QuizHaveQuestionsAsync(int id, CancellationToken ct = default)
        {
            return await SafeQueryAsync(async token =>
                await _db.Domanda.AsNoTracking()
                                 .AnyAsync(q => q.QuizId == id, token)
            , ct);
        }
        public Task<bool> UpdateQuizAsync(QuizDTO request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteQuizByIdAsync(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        //============================= LATO DOMANDE =============================================

        public async Task<QuestionsDTO> GetQuestionsByQuizIdAsync(int quizId, CancellationToken ct = default)
        {
            var questionsDto = new QuestionsDTO
            {
                QuizId = quizId,
                Questions = []
            };

            // Recupera tutte le domande associate al quiz
            var domande = await SafeQueryAsync(async token =>
                await _db.Domanda
                         .AsNoTracking()
                         .Where(d => d.QuizId == quizId)
                         .ToListAsync(token)
            , ct) ?? [];

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
                                   ? [.. d.RisposteGiuste.Split(',', StringSplitOptions.RemoveEmptyEntries)]
                                   : [],

                    WrongAnswers = !string.IsNullOrWhiteSpace(d.RisposteSbagliate)
                                   ? [.. d.RisposteSbagliate.Split(',', StringSplitOptions.RemoveEmptyEntries)]
                                   : []
                };

                questionsDto.Questions.Add(question);
            }

            return questionsDto;
        }

        public async Task<List<int>> AddQuestionsToQuizAsync(QuestionsDTO request, CancellationToken ct = default)
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

            return success ? [.. createdQuestions.Select(q => q.DomandaId)] : [];
        }


        public Task<bool> DeleteQuestionsByIdsAsync(List<int> ids, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateQuestionsAsync(QuestionsDTO request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<int?> CountQuestionsByQuizIdAsync(int quizId, CancellationToken ct= default)
        {
            return await SafeQueryAsync<int?>(async token =>
            {
  
                var count = await _db.Domanda
                    .Where(d => d.QuizId == quizId)
                    .CountAsync(token);

                return count;
            }, ct);
        }

        //============================= LATO QUIZSEED =====================================
        public async Task<int?> CreateQuizSeedAsync(QuizSeedDTO request, CancellationToken ct = default)
        {
            int? quizSeedId = null;

            bool success = await SafeExecuteAsync(async token =>
            {
                var seed = new QuizSeed
                {
                    Nome = request.Name,
                    Modalita = request.Mode,
                    NumeroDomande = request.QuestionNumner,
                    PossibilitaScartoTempo = request.CanUseTimeGap,
                    PossibilitaTornareIndietro = request.CanGoBack,
                    SommaTempoDomande = request.QuestionTotalTime,
                    TempoTotale = request.TotalTime,
                    QuizId = request.QuizId,
                    UserId = request.UserId,                    
                    Pubblico = request.Public
                };

                await _db.QuizSeeds.AddAsync(seed, token);
                await SaveChangesAsync(token);

                quizSeedId = seed.QuizSeedId;
            }, ct);

            return success ? quizSeedId : null;
        }

        public Task<List<QuizSeedDTO?>> GetQuizSeedsByQuizIdAndUserIdAsync(int id, int? userId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateQuizSeedAndDeleteScoreboardRecordsByIdAsync(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteQuizSeedAndDeleteScoreboardRecordsByIdAsync(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }




        //============================= LATO SCOREBOARD =======================================

        public Task<int?> CreateScoreboardRecord(ScoreboardDTO request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }


        //============================= LATO PULL =============================================



    }
}
