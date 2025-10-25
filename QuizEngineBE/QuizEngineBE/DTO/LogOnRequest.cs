namespace QuizEngineBE.DTO
{
    public class LogOnRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;


        public bool CheckFields =>
            !string.IsNullOrWhiteSpace(Username) ||
            !string.IsNullOrWhiteSpace(Password);




    }
}
