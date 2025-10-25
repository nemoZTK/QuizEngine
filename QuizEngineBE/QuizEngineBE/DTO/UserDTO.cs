using Azure;

namespace QuizEngineBE.DTO
{
    public class UserDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? Salt { get; set; }

        public string? Ruolo { get; set; }

        public bool CheckFields =>
            !string.IsNullOrWhiteSpace(Username) ||
            !string.IsNullOrWhiteSpace(Password) ||
            !string.IsNullOrWhiteSpace(Email);

    }

}
