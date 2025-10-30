using Azure;
using Newtonsoft.Json;

namespace QuizEngineBE.DTO.UserSpace
{
    /// <summary>
    /// DTO dell' utente, contiene da descrizione dell'utente e un metodo per controllare i campi
    /// </summary>
    public class UserDTO
    {
        public int? Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Salt { get; set; } = "sale&pepe";

        public string Ruolo { get; set; } = "1";


        [JsonIgnore]
        public bool CheckFields =>
            !string.IsNullOrWhiteSpace(Username) ||
            !string.IsNullOrWhiteSpace(Password) ||
            !string.IsNullOrWhiteSpace(Email);

    }

}
