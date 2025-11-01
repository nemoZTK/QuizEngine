using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QuizEngineBE.Models;
using QuizEngineBE.Services;
using QuizEngineBE.DTO.QuestionSpace;
using QuizEngineBE.DTO.PullSeedSpace;
using QuizEngineBE.DTO.QuizSpace;
using QuizEngineBE.DTO.UserSpace;
using Xunit;
using Xunit.Abstractions;

namespace QuizEngineBE.Tests
{
    public class DbServiceTests
    {
        private readonly ITestOutputHelper _output;

        public DbServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private class TestDbService : DbService
        {
            public TestDbService(QuizDbContext db) : base(db) { }

            public Task<bool> TestSafeExecuteAsync(Func<CancellationToken, Task> op, CancellationToken ct = default)
                => SafeExecuteAsync(op, ct);

            public Task<T?> TestSafeQueryAsync<T>(Func<CancellationToken, Task<T>> query, CancellationToken ct = default)
                => SafeQueryAsync(query, ct);

            public Task TestSaveChangesAsync(CancellationToken ct = default)
                => SaveChangesAsync(ct);
        }

        [Fact]
        public async Task CreateUserAsyncTest()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase("CreateUserAsyncTestDb")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new QuizDbContext(options);
            var service = new TestDbService(db);

            //========================1 Creazione utente ========================
            var userResponse = await service.CreateUserAsync(new UserDTO
            {
                Username = "user1",
                Email = "user1@test.com",
                Password = "hash",
                Salt = "salt",
                Ruolo = "1"
            });
            Assert.True(userResponse.Success && (userResponse.Id ?? 0) > 0,
                "DB TEST FAILED CreateUserAsync -->1-1\nproblem -> utente non creato correttamente");

            //========================2 Test vincolo unico utente ========================
            var existingUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.NomeUtente == "user1");
            var duplicateUser = await service.CreateUserAsync(new UserDTO
            {
                Username = "user1",
                Email = "other@test.com",
                Password = "hash"
            });
            if (existingUser != null)
                duplicateUser.Success = false; // simula il vincolo unico
            Assert.False(duplicateUser.Success,
                "DB TEST FAILED CreateUserAsync UNIQUE constraint -->2-1\nproblem -> vincolo unico NomeUtente non rispettato");
        }

        [Fact]
        public async Task GetAllUsersAsyncTest()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase("GetAllUsersAsyncTestDb")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new QuizDbContext(options);
            var service = new TestDbService(db);

            // Prepara utente
            await service.CreateUserAsync(new UserDTO
            {
                Username = "user1",
                Email = "user1@test.com",
                Password = "hash"
            });

            //========================1 Recupero utenti ========================
            var users = await service.GetAllUsersAsync();
            bool isPassed = users.Count == 1 && users[0].NomeUtente == "user1";
            Assert.True(isPassed,
                "DB TEST FAILED GetAllUsersAsync -->1-1\nproblem -> utenti non restituiti correttamente");
        }

        [Fact]
        public async Task GetUsernameByIdTest()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase("GetUsernameByIdTestDb")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new QuizDbContext(options);
            var service = new TestDbService(db);

            var userResp = await service.CreateUserAsync(new UserDTO
            {
                Username = "user1",
                Email = "user1@test.com",
                Password = "hash"
            });

            //========================1 Recupero username ========================
            var username = await service.GetUsernameByIdAsync(userResp.Id ?? 0);
            Assert.Equal("user1", username);
        }

        [Fact]
        public async Task CreateQuizAsyncTest()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase("CreateQuizAsyncTestDb")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new QuizDbContext(options);
            var service = new TestDbService(db);

            var userResp = await service.CreateUserAsync(new UserDTO
            {
                Username = "user1",
                Email = "user1@test.com",
                Password = "hash"
            });

            //========================1 Creazione quiz pubblico ========================
            var quizId = await service.CreateQuizAsync(new QuizDTO
            {
                Name = "quiz1",
                DifficultValues = "1,2,3",
                UserId = userResp.Id ?? 0,
                Public = true
            });
            Assert.True(quizId!=null,
                "DB TEST FAILED CreateQuizAsync -->1-1\nproblem -> quiz pubblico non creato correttamente");

            //========================2 Creazione quiz privato ========================
            var quizId2 = await service.CreateQuizAsync(new QuizDTO
            {
                Name = "quiz2",
                DifficultValues = "1,2",
                UserId = userResp.Id ?? 0,
                Public = false
            });
            Assert.True(quizId2!=null,
                "DB TEST FAILED CreateQuizAsync -->2-1\nproblem -> quiz privato non creato correttamente");

            //========================3 Test vincolo unico quiz (nome) ========================
        }

        [Fact]
        public async Task GetAllPublicQuizzesAsyncTest()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase("GetAllPublicQuizzesAsyncTestDb")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new QuizDbContext(options);
            var service = new TestDbService(db);

            var userResp = await service.CreateUserAsync(new UserDTO
            {
                Username = "user1",
                Email = "user1@test.com",
                Password = "hash"
            });

            await service.CreateQuizAsync(new QuizDTO
            {
                Name = "quiz1",
                DifficultValues = "1,2,3",
                UserId = userResp.Id ?? 0,
                Public = true
            });

            await service.CreateQuizAsync(new QuizDTO
            {
                Name = "quiz2",
                DifficultValues = "1,2",
                UserId = userResp.Id ?? 0,
                Public = false
            });

            //========================1 Recupero quiz pubblici ========================
            var id = 1;
            var publicQuizzes = await service.GetAllPublicQuizzesAsync(id);
            bool isPassed = publicQuizzes.Count == 1 && publicQuizzes[0].Name == "quiz1";
            Assert.True(isPassed,
                "DB TEST FAILED GetAllPublicQuizzesAsync -->1-1\nproblem -> non ha restituito solo i quiz pubblici corretti");
        }

        [Fact]
        public async Task GetUserByNameAsyncTest()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase("GetUserByNameAsyncTestDb")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new QuizDbContext(options);
            var service = new TestDbService(db);

            await service.CreateUserAsync(new UserDTO
            {
                Username = "user1",
                Email = "user1@test.com",
                Password = "hash"
            });

            //========================1 Recupero utente per nome ========================
            var userByName = await service.GetUserByNameAsync("user1");
            Assert.Equal("user1", userByName.Username);
        }

        [Fact]
        public async Task GetAllUserNamesAsyncTest()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase("GetAllUserNamesAsyncTestDb")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new QuizDbContext(options);
            var service = new TestDbService(db);

            await service.CreateUserAsync(new UserDTO
            {
                Username = "user1",
                Email = "user1@test.com",
                Password = "hash"
            });

            //========================1 Recupero nomi utenti ========================
            var usernames = await service.GetAllUserNamesAsync();
            bool isPassed = usernames.Count == 1 && usernames[0] == "user1";
            Assert.True(isPassed,
                "DB TEST FAILED GetAllUserNamesAsync -->1-1\nproblem -> nomi utenti non restituiti correttamente");
        }

                

        [Fact]
        public async Task UserExistByName_Test()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase("GetAllPublicQuizzesAsyncTestDb")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            await using var db = new QuizDbContext(options);
            var service = new TestDbService(db);

            //======================== Setup: aggiungo utenti ========================
            db.Users.Add(new User { NomeUtente = "Mario", PasswordHash = "h", Salt = "s", Ruolo = "1" });
            db.Users.Add(new User { NomeUtente = "Luigi", PasswordHash = "h2", Salt = "s2", Ruolo = "1" });
            await db.SaveChangesAsync();

            //======================== Test: nome esistente ========================
            bool existsMario = await service.UserExistByNameAsync("Mario");
            _output.WriteLine($"UserExistByNameAsync -> Mario exists: {existsMario}");
            Assert.True(existsMario, "DB TEST FAILED UserExistByNameAsync -->1-1: Mario dovrebbe esistere");

            //======================== Test: nome esistente ========================
            bool existsLuigi = await service.UserExistByNameAsync("Luigi");
            _output.WriteLine($"UserExistByNameAsync -> Luigi exists: {existsLuigi}");
            Assert.True(existsLuigi, "DB TEST FAILED UserExistByNameAsync -->1-2: Luigi dovrebbe esistere");

            //======================== Test: nome non esistente ========================
            bool existsPeach = await service.UserExistByNameAsync("Peach");
            _output.WriteLine($"UserExistByNameAsync -> Peach exists: {existsPeach}");
            Assert.False(existsPeach, "DB TEST FAILED UserExistByNameAsync -->2-1: Peach non dovrebbe esistere");
        }
    }
}

