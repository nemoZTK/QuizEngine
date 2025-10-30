using QuizEngineBE.DTO.SessionSpace;

namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// service che si occupa dell' avvio e della conclusione di una sessione, con relativo assegnamento dei punti
    /// </summary>
    public interface ISessionService
    {

        /// <summary>
        /// da inizio ad una sessione di quiz
        /// </summary>
        /// <returns>una SessionResponse</returns>
        SessionResponse StartSession(SessionRequest request);

        /// <summary>
        /// termina una sessione e assegna il punteggio 
        /// </summary>
        /// <returns></returns>
        SessionResponse EndSession(SessionResponse request);
    }
}
