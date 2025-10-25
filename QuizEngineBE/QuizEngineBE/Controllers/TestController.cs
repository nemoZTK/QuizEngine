using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizEngineBE.Services;

namespace QuizEngineBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(QuizEngineService engine, SecurityService sec,UserService usr) : ControllerBase
    {

        private readonly QuizEngineService _engine=engine;
        private readonly SecurityService _sec=sec;
        private readonly UserService _usr = usr;


        [HttpGet("encrypt")]
        public async Task<IActionResult> Encrypt([FromQuery] string word)
        {
            return Ok(_sec.EncryptSHA256xBase64(word));
        }



    }
}
