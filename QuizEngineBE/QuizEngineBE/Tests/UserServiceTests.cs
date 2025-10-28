using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using QuizEngineBE.DTO;
using QuizEngineBE.Models;
using QuizEngineBE.Services;
using Xunit;
using Xunit.Abstractions;

namespace QuizEngineBE.Tests
{
    public class UserServiceTests
    {
        private readonly ITestOutputHelper _output;

        public UserServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // helper: crea SecurityService con settings in-memory (necessario per usare UserService reale)
        private SecurityService BuildSecurityService()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"Jwt:Key", "chiave_molto_lunga_che_serve_ad_aprire_un_portale_altrettanto_grande"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return new SecurityService(config);
        }

        // helper: crea QuizDbContext InMemory (ignora warning transazioni in memoria)
        private QuizDbContext BuildInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new QuizDbContext(options);
        }

        [Fact]
        public async Task GetUsers()
        {
            //======================== Setup ========================
            string dbName = nameof(GetUsers) + Guid.NewGuid();
            await using var db = BuildInMemoryDb(dbName);
            var dbService = new DbService(db);
            var sec = BuildSecurityService();
            var service = new UserService(dbService, sec);

            //========================1 Popolo DB con 2 utenti ========================
            db.Users.Add(new User { NomeUtente = "Mario", PasswordHash = "h", Salt = "s", Ruolo = "1" });
            db.Users.Add(new User { NomeUtente = "Luigi", PasswordHash = "h2", Salt = "s2", Ruolo = "1" });
            await db.SaveChangesAsync();

            //---- azione
            var users = await service.GetUsers();


            //---- asserzione
            bool isPassed = users.Count == 2 &&
                            users.Any(u => u.NomeUtente == "Mario") &&
                            users.Any(u => u.NomeUtente == "Luigi");

            Assert.True(isPassed, "USER TEST FAILED GetUsers -->1-1" + "\n" +
                "problem -> numero o nomi utenti errati");
        }

        [Fact]
        public async Task GetUserNames()
        {
            //======================== Setup ========================
            string dbName = nameof(GetUserNames) + Guid.NewGuid();
            await using var db = BuildInMemoryDb(dbName);
            var dbService = new DbService(db);
            var sec = BuildSecurityService();
            var service = new UserService(dbService, sec);

            //========================1 Popolo DB ========================
            db.Users.Add(new User { NomeUtente = "Pippo", PasswordHash = "h", Salt = "s", Ruolo = "1" });
            db.Users.Add(new User { NomeUtente = "Pluto", PasswordHash = "h2", Salt = "s2", Ruolo = "1" });
            await db.SaveChangesAsync();

            //---- azione
            var names = await service.GetUserNames();


            //---- asserzione
            bool isPassed = names.Contains("Pippo") && names.Contains("Pluto");
            Assert.True(isPassed, "USER TEST FAILED GetUserNames -->1-1" + "\n" +
                "problem -> nomi utenti mancanti o errati");
        }

        [Fact]
        public async Task CreateNewUser()
        {
            //======================== Setup ========================
            string dbName = nameof(CreateNewUser) + Guid.NewGuid();
            await using var db = BuildInMemoryDb(dbName);
            var dbService = new DbService(db);
            var sec = BuildSecurityService();
            var service = new UserService(dbService, sec);

            //========================1 Missing fields deve restituire MissingFields ========================
            var emptyDto = new UserDTO { Username = "", Password = "", Email = "" };
            var respMissing = await service.CreateNewUser(emptyDto);

            Assert.False(respMissing.Success, "USER TEST FAILED CreateNewUser -->1-1" + "\n" +
                "problem -> expected MissingFields ma Success true");
            Assert.True(respMissing.Message.Contains("campi mancanti"), "USER TEST FAILED CreateNewUser -->1-2" + "\n" +
                "problem -> message non contiene 'campi mancanti'");

            //========================2 Creazione valida ========================
            var dto = new UserDTO
            {
                Username = "newuser",
                Password = "pwd",
                Email = "a@b.c"
            };

            var resp = await service.CreateNewUser(dto);

            _output.WriteLine("CreateNewUser -> created Id: " + resp.Id + " Success: " + resp.Success + " Message: " + resp.Message);

            bool isPassed = resp.Success && (resp.Id ?? 0) > 0;
            Assert.True(isPassed, "USER TEST FAILED CreateNewUser -->2-1" + "\n" +
                "problem -> utente non creato correttamente");

            //---- controllo che il db contenga l'utente e che il nome sia corretto
            var persisted = await db.Users.FirstOrDefaultAsync(u => u.NomeUtente == "newuser");
            Assert.True(persisted != null, "USER TEST FAILED CreateNewUser -->2-2" + "\n" +
                "problem -> utente non trovato nel DB dopo CreateNewUser");

            //========================3 Creazione duplicato ========================
            // Creo un DTO con lo stesso username di prima
            var dtoDup = new UserDTO
            {
                Username = "newuser",
                Password = "pwd2",
                Email = "b@c.d"
            };

            // Provo a creare l'utente duplicato
            var respDup = await service.CreateNewUser(dtoDup);

            //---- log
            _output.WriteLine("CreateNewUser -> duplicate attempt Success: " + respDup.Success + " Message: " + respDup.Message);

            //---- asserzioni
            Assert.False(respDup.Success, "USER TEST FAILED CreateNewUser -->3-1" + "\n" +
                "problem -> duplicazione username non gestita come expected");
            Assert.True(respDup.Message.Contains("Username già esistente"),
                "USER TEST FAILED CreateNewUser -->3-2" + "\n" +
                "problem -> messaggio duplicato non corretto");

        }

        [Fact]
        public async Task IsValidRequest()
        {
            //======================== Setup ========================
            string dbName = nameof(IsValidRequest) + Guid.NewGuid();
            await using var db = BuildInMemoryDb(dbName);
            var dbService = new DbService(db);
            var sec = BuildSecurityService();
            var service = new UserService(dbService, sec);

            //========================1 Missing fields nel login ========================
            var badLogin = new LogOnRequest { Username = "", Password = "" };
            var respBad = await service.IsValidRequest(badLogin);

            Assert.False(respBad.Success, "USER TEST FAILED IsValidRequest -->1-1" + "\n" +
                "problem -> expected MissingFields ma Success true");
            Assert.True(respBad.Message.Contains("campi mancanti"), "USER TEST FAILED IsValidRequest -->1-2" + "\n" +
                "problem -> message non contiene 'campi mancanti'");

            //========================2 Utente presente ma password sbagliata ========================
            var salt = sec.GenerateSalt();
            var pwdHash = sec.EncryptSHA256xBase64("correctPwd" + salt);

            var user = new User
            {
                NomeUtente = "loginUser",
                PasswordHash = pwdHash,
                Salt = salt,
                Ruolo = "1"
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var wrongLogin = new LogOnRequest { Username = "loginUser", Password = "badPwd" };
            var respWrong = await service.IsValidRequest(wrongLogin);

            Assert.False(respWrong.Success, "USER TEST FAILED IsValidRequest -->2-1" + "\n" +
                "problem -> password sbagliata non rilevata");
            Assert.True(respWrong.Message.Contains("username o Password sbagliate"), "USER TEST FAILED IsValidRequest -->2-2" + "\n" +
                "problem -> message non contiene 'username o Password sbagliate'");

            //========================3 Login corretto ========================
            var goodLogin = new LogOnRequest { Username = "loginUser", Password = "correctPwd" };
            var respGood = await service.IsValidRequest(goodLogin);

            Assert.True(respGood.Success, "USER TEST FAILED IsValidRequest -->3-1" + "\n" +
                "problem -> login corretto non riconosciuto");
            Assert.True(respGood.Id.HasValue && respGood.Id.Value > 0, "USER TEST FAILED IsValidRequest -->3-2" + "\n" +
                "problem -> Id non valorizzato nel response");
        }

        [Fact]
        public void GenerateJwtToken()
        {
            //======================== Setup ========================
            var sec = BuildSecurityService();
            var db = BuildInMemoryDb(nameof(GenerateJwtToken) + Guid.NewGuid());
            var dbService = new DbService(db);
            var service = new UserService(dbService, sec);

            //========================1 Generazione token ========================================
            string token = service.GenerateJwtToken("mimmo");

            bool isPassed = !string.IsNullOrEmpty(token);
            Assert.True(isPassed, "USER TEST FAILED GenerateJwtToken -->1-1" + "\n" +
                "problem -> token generato è vuoto");
        }

        [Fact]
        public async Task GetUsernameById()
        {
            //======================== Setup ========================
            string dbName = nameof(GetUsernameById) + Guid.NewGuid();
            await using var db = BuildInMemoryDb(dbName);
            var dbService = new DbService(db);
            var sec = BuildSecurityService();
            var service = new UserService(dbService, sec);

            //========================1 Creo utente e controllo recupero ========================
            db.Users.Add(new User { NomeUtente = "whoami", PasswordHash = "h", Salt = "s", Ruolo = "1" });
            await db.SaveChangesAsync();

            var u = await db.Users.FirstAsync();
            var name = await service.GetUsernameById(u.UserId);


            //---- asserzione con messaggio di errore personalizzato
            Assert.True(
                name == "whoami",
                "USER TEST FAILED GetUsernameById -->1-1" + "\n" +
                "problem -> nome utente recuperato errato o non trovato nel DB" + "\n" +
                "expected: whoami" + "\n" +
                "actual: " + name
            );

            //========================2 Id non esistente deve restituire stringa vuota ========================
            var missing = await service.GetUsernameById(-999);
            Assert.True(string.IsNullOrEmpty(missing), "USER TEST FAILED GetUsernameById -->2-1" + "\n" +
                "problem -> id inesistente dovrebbe restituire stringa vuota");
        }

        [Fact]
        public void IsUserAuthenticated()
        {
            //======================== Setup ========================
            var sec = BuildSecurityService();
            var db = BuildInMemoryDb(nameof(IsUserAuthenticated) + Guid.NewGuid());
            var dbService = new DbService(db);
            var service = new UserService(dbService, sec);

            //========================1 token null deve avvisare ========================
            var authNull = service.IsUserAuthenticated("mimmo", null);
            Assert.False(authNull.success, "USER TEST FAILED IsUserAuthenticated -->1-1" + "\n" +
                "problem -> token null non ha fallito");
            Assert.True(authNull.message.Contains("almeno passalo un token dai"), "USER TEST FAILED IsUserAuthenticated -->1-2" + "\n" +
                "problem -> message token null non corrisponde");

            //========================2 token valido ========================
            string token = sec.GenerateJwtToken("mimmo");
            var authOk = service.IsUserAuthenticated("mimmo", token);

            Assert.True(authOk.success, "USER TEST FAILED IsUserAuthenticated -->2-1" + "\n" +
                "problem -> token valido non riconosciuto");

            //========================3 token valido ma username diverso ========================
            var authMismatch = service.IsUserAuthenticated("other", token);
            Assert.False(authMismatch.success, "USER TEST FAILED IsUserAuthenticated -->3-1" + "\n" +
                "problem -> token valido per user diverso non ha fallito");
            Assert.True(authMismatch.message.Contains("Username non corrispondente"), "USER TEST FAILED IsUserAuthenticated -->3-2" + "\n" +
                "problem -> messaggio non contiene 'Username non corrispondente'");
        }
    }
}
