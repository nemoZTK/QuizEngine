using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using QuizEngineBE.Services;
using Xunit;
using Xunit.Abstractions;

namespace QuizEngineBE.Tests
{
    public class SecurityServiceTests
    {
        private readonly SecurityService _service;
        private readonly ITestOutputHelper _output;

        public SecurityServiceTests(ITestOutputHelper output)
        {
            _output = output;
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"Jwt:Key", "chiave_molto_lunga_che_serve_ad_aprire_un_portale_altrettanto_grande_che_si_trova_in_un_isola_sperduta_nel_pacifico_meridionale"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _service = new SecurityService(config);
        }

        [Fact]
        public void EncryptSHA256xBase64()
        {
            //======================== dati 2 elementi uguali deve produrre 2 hash uguali =================================
            var hash1 = _service.EncryptSHA256xBase64("test");
            var hash2 = _service.EncryptSHA256xBase64("test");
            bool isPassed = string.Equals(hash1, hash2);
            Assert.True(isPassed, "SECURITY TEST FAILED EncryptSHA256xBase64 -->1-1");

            //======================== dati 2 elementi diversi deve produrre 2 hash diversi ==============================
            hash2 = _service.EncryptSHA256xBase64("test2");
            isPassed = string.Equals(hash1, hash2);
            Assert.False(isPassed, "SECURITY TEST FAILED EncryptSHA256xBase64 -->1-2");
        }

        [Fact]
        public void ValidateJwtTokenForUser()
        {
            var tokenVaidity = _service.GenerateJwtToken("mimmo");
            bool isPassed;
            string message;

            //========================1 dato nome diverso la verifica del token deve avvertire ==========================
            (isPassed, message) = _service.ValidateJwtTokenForUser("franco", tokenVaidity);
            Assert.False(isPassed, "SECURITY TEST FAILED ValidateJwtTokenForUser -->1-1");
            Assert.True(message.Contains("Username non corrispondente al Token"), "SECURITY TEST FAILED ValidateJwtTokenForUser -->1-2" +
                "\nproblem -> " + message + " expected-> Username non corrispondente al Token");

            //========================2 dato nome giusto deve passare ================================================
            (isPassed, message) = _service.ValidateJwtTokenForUser("mimmo", tokenVaidity);
            Assert.True(isPassed, "SECURITY TEST FAILED ValidateJwtTokenForUser -->2-1" +
                "\nproblem -> " + message);
            isPassed = message.Equals("Token valido");
            Assert.True(isPassed, "SECURITY TEST FAILED ValidateJwtTokenForUser -->2-2" +
                "\nproblem -> " + message + " expected -> Token valido");

            //========================3 username o token non forniti devono avvisare ============================
            //---- username errato ----------------------------------------------------------------------------
            (isPassed, message) = _service.ValidateJwtTokenForUser("", tokenVaidity);
            Assert.False(isPassed, "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-1" +
                "\nproblem -> " + message);
            Assert.True(message.Contains("Token o username non forniti"), "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-2" +
                "\nproblem -> " + message + " expected-> Token o username non forniti");

            (isPassed, message) = _service.ValidateJwtTokenForUser(null, tokenVaidity);
            Assert.False(isPassed, "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-3" +
                "\nproblem -> " + message);
            Assert.True(message.Contains("Token o username non forniti"), "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-4" +
                "\nproblem -> " + message + " expected-> Token o username non forniti");

            (isPassed, message) = _service.ValidateJwtTokenForUser(" ", tokenVaidity);
            Assert.False(isPassed, "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-5" +
                "\nproblem -> " + message);
            Assert.True(message.Contains("Token o username non forniti"), "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-6" +
                "\nproblem -> " + message + " expected-> Token o username non forniti");

            //---- token errato ----------------------------------------------------------------------------
            (isPassed, message) = _service.ValidateJwtTokenForUser("franco", " ");
            Assert.False(isPassed, "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-7" +
                "\nproblem -> " + message);
            Assert.True(message.Contains("Token o username non forniti"), "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-8" +
                "\nproblem -> " + message + " expected-> Token o username non forniti");

            (isPassed, message) = _service.ValidateJwtTokenForUser("franco", null);
            Assert.False(isPassed, "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-9" +
                "\nproblem -> " + message);
            Assert.True(message.Contains("Token o username non forniti"), "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-10" +
                "\nproblem -> " + message + " expected-> Token o username non forniti");

            (isPassed, message) = _service.ValidateJwtTokenForUser("franco", "");
            Assert.False(isPassed, "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-11" +
                "\nproblem -> " + message);
            Assert.True(message.Contains("Token o username non forniti"), "SECURITY TEST FAILED ValidateJwtTokenForUser -->3-12" +
                "\nproblem -> " + message + " expected-> Token o username non forniti");
        }
        

        [Fact]
        public void GetBearerToken()
        {
            //================================= dato un bearer token con la scritta "Bearer <token>" mi deve restituire il token ================================= 
            string bearerT = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9."+
            "eyJzdWIiOiJnaXVzZXBwZVNpbW9uZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29m"+
            "dC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IjEiLCJqdGkiOi"+
            "IxMjJjYTk3YS1lMjlhLTQxMjQtYWIwYS0zZTIxYzRhNGQ0NjQiLCJleHAiOjE3NjE0ND"+
            "AyNjAsImlzcyI6IlF1aXpFbmdpbmVCRSIsImF1ZCI6IlF1aXpFbmdpbmVDbGllbnQifQ.kZwJv-pECLxBQ6GF1scWftC5oKEi9IB87OL50FSYBGY";
            string bearerTfinal = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
            "eyJzdWIiOiJnaXVzZXBwZVNpbW9uZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29m" +
            "dC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IjEiLCJqdGkiOi" +
            "IxMjJjYTk3YS1lMjlhLTQxMjQtYWIwYS0zZTIxYzRhNGQ0NjQiLCJleHAiOjE3NjE0ND" +
            "AyNjAsImlzcyI6IlF1aXpFbmdpbmVCRSIsImF1ZCI6IlF1aXpFbmdpbmVDbGllbnQifQ.kZwJv-pECLxBQ6GF1scWftC5oKEi9IB87OL50FSYBGY";

            string bearerTproduced = _service.GetBearerToken(bearerT);
            bool isPassed = bearerTproduced.Equals(bearerTfinal);
            Assert.True(isPassed, "SECURITY TEST FAILED GetBearerToken -->1-1" +
                "\nproblem --> bearerTproduced != bearerTfinal");

            isPassed = (bearerTproduced + "cose").Equals(bearerTfinal);
            Assert.False(isPassed, "SECURITY TEST FAILED GetBearerToken -->1-2" +
                "\nproblem --> bearerTproduced sembra uguale a bearerTfinal");

            //========================2 Token null, vuoto o spazi deve restituire null========================
            Assert.True(_service.GetBearerToken(null) == null, "SECURITY TEST FAILED GetBearerToken -->2-1" +
                "\nproblem -> token null non restituisce null");

            Assert.True(_service.GetBearerToken("") == null, "SECURITY TEST FAILED GetBearerToken -->2-2" +
                "\nproblem -> token vuoto non restituisce null");

            Assert.True(_service.GetBearerToken(" ") == null, "SECURITY TEST FAILED GetBearerToken -->2-3" +
                "\nproblem -> token spazi non restituisce null");

            //========================3 Token senza 'Bearer ' deve restituire il token così com'è========================
            string rawToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test";
            string tokenProduced = _service.GetBearerToken(rawToken);
            Assert.True(tokenProduced == rawToken, "SECURITY TEST FAILED GetBearerToken -->3-1" +
                "\nproblem -> token senza 'Bearer ' non restituisce token corretto");

            //========================4 Token con 'bearer ' minuscolo deve rimuovere correttamente========================
            string bearerLower = "bearer " + rawToken;
            tokenProduced = _service.GetBearerToken(bearerLower);
            Assert.True(tokenProduced == rawToken, "SECURITY TEST FAILED GetBearerToken -->4-1" +
                "\nproblem -> token con 'bearer ' minuscolo non restituisce token pulito");

            //========================5 Token con spazi prima/dopo deve essere trim========================
            string bearerWithSpaces = "   Bearer " + rawToken + "   ";
            tokenProduced = _service.GetBearerToken(bearerWithSpaces);
            Assert.True(tokenProduced == rawToken, "SECURITY TEST FAILED GetBearerToken -->5-1" +
                "\nproblem -> token con spazi prima/dopo non restituisce token trim");

        }

        [Fact]
        public void GenerateJwtToken()
        {
            //========================1 Generazione token base========================================
            string token = _service.GenerateJwtToken("mimmo", "1", 5);
            bool isPassed = !string.IsNullOrEmpty(token);
            Assert.True(isPassed, "SECURITY TEST FAILED GenerateJwtToken -->1-1" +
                "\nproblem -> token generato è vuoto");

            //========================2 Due token consecutivi devono essere diversi per Jti========================
            string token2 = _service.GenerateJwtToken("mimmo", "1", 5);
            isPassed = token != token2;
            Assert.True(isPassed, "SECURITY TEST FAILED GenerateJwtToken -->2-1" +
                "\nproblem -> due token consecutivi sono uguali, devono differire per Jti");

            //========================3 Token con ruolo personalizzato========================
            string tokenRole = _service.GenerateJwtToken("mimmo", "admin", 5);
            isPassed = !string.IsNullOrEmpty(tokenRole);
            Assert.True(isPassed, "SECURITY TEST FAILED GenerateJwtToken -->3-1" +
                "\nproblem -> token con ruolo personalizzato è vuoto");

            //========================4 Verifica che il token decodificato contenga username corretto========================
            var (success, message) = _service.ValidateJwtTokenForUser("mimmo", tokenRole);
            Assert.True(success, "SECURITY TEST FAILED GenerateJwtToken -->4-1" +
                "\nproblem -> token generato non valido: " + message);

            //========================5 Token con expirazione breve deve scadere========================
            string tokenShort = _service.GenerateJwtToken("mimmo", "1", 0); // scadenza immediata
            System.Threading.Thread.Sleep(1000); // attesa 1 sec per essere sicuri
            (success, message) = _service.ValidateJwtTokenForUser("mimmo", tokenShort);
            Assert.False(success && message == "Token valido", "SECURITY TEST FAILED GenerateJwtToken -->5-1" +
                "\nproblem -> token dovrebbe essere scaduto ma risulta valido");
        }




        [Fact]
        public void GenerateSalt()
        {
            //========================1 Genera un salt di lunghezza corretta========================
            string salt = _service.GenerateSalt();
            bool isPassed = !string.IsNullOrEmpty(salt);
            Assert.True(isPassed, "SECURITY TEST FAILED GenerateSalt -->1-1"+
                "\nproblem -> salt è vuoto");

            // lunghezza base64 minima attesa
            int expectedMinLength = (int)Math.Ceiling(16 * 4.0 / 3.0);
            isPassed = salt.Length >= expectedMinLength;
            Assert.True(isPassed, "SECURITY TEST FAILED GenerateSalt -->1-2"+
                $"\nproblem -> lunghezza salt {salt.Length}, expected almeno {expectedMinLength}");

            //========================2 Due salt consecutivi devono essere diversi========================
            string salt2 = _service.GenerateSalt();
            isPassed = salt != salt2;
            Assert.True(isPassed, "SECURITY TEST FAILED GenerateSalt -->2-1"+
                $"\nproblem -> due salt consecutivi sono uguali:\n-->{salt} \n\te  \n-->{salt2}");

            //========================3 Salt con lunghezza personalizzata========================
            int customLength = 32;
            string saltCustom = _service.GenerateSalt(customLength);
            int expectedMinLengthCustom = (int)Math.Ceiling(customLength * 4.0 / 3.0);
            isPassed = saltCustom.Length >= expectedMinLengthCustom;
            Assert.True(isPassed, "SECURITY TEST FAILED GenerateSalt -->3-1"+
                $"\nproblem -> lunghezza salt personalizzato {saltCustom.Length}, expected almeno {expectedMinLengthCustom}");
        }

    }
}
