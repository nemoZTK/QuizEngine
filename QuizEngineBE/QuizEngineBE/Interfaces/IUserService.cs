using QuizEngineBE.DTO.QuestionSpace;
using QuizEngineBE.DTO.QuizSpace;
using QuizEngineBE.DTO.UserSpace;

namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// service che gestisce gli utenti, il loro ottenimento, la loro creazione, modifica e validazione, si occupa anche di creare il jwt token.
    /// <br/>è l'unico a parlare con SecurityService
    /// <br/> usa DbService e SecurityService
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// crea un nuovo utente data una UserDTO
        /// </summary>
        /// <returns>una UserResponse</returns>
        Task<UserResponse> CreateNewUser(UserDTO user);
        /// <summary>
        /// prova a fare il login data una LogOnRequest
        /// </summary>
        /// <returns>una UserResponse</returns>
        Task<UserResponse> TryToDoLogin(LogOnRequest loginRequest);

        /// <summary>
        /// genra un jwt token dato uno username
        /// </summary>
        /// <returns>stringa col jwt token</returns>
        string GenerateJwtToken(string username);
        //TODO: ci starebbe l'aggiunta di ruoli (è tutto già pronto nel db, ma per ora son tutti ruolo "1")

        /// <summary>
        /// ottiene un username dato il suo userId
        /// </summary>
        /// <returns>stringa con username</returns>
        Task<string> GetUsernameById(int id);


    }
}
