using Microsoft.Extensions.Configuration;
using QuizEngineBE.Services;
using Xunit;
using Xunit.Abstractions;

namespace QuizEngineBE.Tests
{
    public class AppSettingsValidationTest
    {
        private readonly IConfiguration _config;
        private readonly ITestOutputHelper _output;

        public AppSettingsValidationTest(ITestOutputHelper output)
        {
            _output = output;

            // Qui caricamento reale dell'appsettings.json
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        }

        [Fact]
        public void AppSettingsValidation()
        {
            bool isPassed;

            //======================== JWT ========================
            isPassed = !string.IsNullOrWhiteSpace(_config["Jwt:Key"]);
            Assert.True(isPassed, "SECURITY TEST FAILED AppSettingsValidation 1-1--> JWT Key mancante o vuota");
            isPassed = _config["Jwt:Key"].Length>=32;
            Assert.True(isPassed, "SECURITY TEST FAILED AppSettingsValidation 1-2-->"
                +"\n JWT troppo corta min 32 fornito:" +_config["Jwt:Key"].Length);
            isPassed = !string.IsNullOrWhiteSpace(_config["Jwt:Issuer"]);
            Assert.True(isPassed, "SECURITY TEST FAILED AppSettingsValidation 1-3--> JWT Issuer mancante o vuoto");

            isPassed = !string.IsNullOrWhiteSpace(_config["Jwt:Audience"]);
            Assert.True(isPassed, "SECURITY TEST FAILED AppSettingsValidation 1-4--> JWT Audience mancante o vuoto");

            //======================== Serilog ========================
            isPassed = !string.IsNullOrWhiteSpace(_config["Serilog:MinimumLevel"]);
            Assert.True(isPassed, "SERILOG TEST FAILED AppSettingsValidation 2-1--> Serilog MinimumLevel mancante o vuoto");

            isPassed = !string.IsNullOrWhiteSpace(_config["Serilog:WriteTo:0:Name"]);
            Assert.True(isPassed, "SERILOG TEST FAILED AppSettingsValidation 2-2--> Serilog WriteTo[0] Name mancante o vuoto");

            isPassed = !string.IsNullOrWhiteSpace(_config["Serilog:WriteTo:1:Name"]);
            Assert.True(isPassed, "SERILOG TEST FAILED AppSettingsValidation 2-3--> Serilog WriteTo[1] Name mancante o vuoto");

            isPassed = !string.IsNullOrWhiteSpace(_config["Serilog:Properties:Application"]);
            Assert.True(isPassed, "SERILOG TEST FAILED AppSettingsValidation 2-4--> Serilog Properties Application mancante o vuoto");

            //======================== ConnectionStrings ========================
            isPassed = !string.IsNullOrWhiteSpace(_config.GetConnectionString("QuizDb"));
            Assert.True(isPassed, "CONNECTIONSTRINGS TEST FAILED AppSettingsValidation 3-1--> ConnectionStrings:QuizDb mancante o vuota");
        
        
        
        }
    }
}
