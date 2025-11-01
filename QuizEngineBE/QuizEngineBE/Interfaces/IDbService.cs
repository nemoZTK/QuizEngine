using QuizEngineBE.DTO.PullSeedSpace;
using QuizEngineBE.DTO.QuestionSpace;
using QuizEngineBE.DTO.QuizSpace;
using QuizEngineBE.DTO.ScoreboardSpace;
using QuizEngineBE.DTO.UserSpace;
using QuizEngineBE.Models;

namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// interfaccia del DbService, colui che conosce come costruire i model partendo da DTO in input.
    /// si occupa di fare tutte le query e le insert nel db, usando DbBaseService
    /// <br/> usa DbBaseService 
    /// </summary>
    public interface IDbService
    {

        //========================= LATO UTENTE =======================================

        /// <summary>
        /// aggiunge un nuovo utente nel db
        /// </summary>
        /// <returns>una user response</returns>
        Task<UserResponse> CreateUserAsync(UserDTO utente, CancellationToken ct);
        
        /// <summary>
        /// ottiene un userDTO partendo dal nome utente
        /// </summary>
        /// <returns>un userDTO</returns>
        Task<UserDTO> GetUserByNameAsync(string name, CancellationToken ct);
        
        /// <summary>
        /// controlla se un utente esista o meno nel db dato il suo presunto nome
        /// </summary>
        /// <returns>se ho trovato l'utente o meno (bool)</returns>
        Task<bool> UserExistByNameAsync(string name, CancellationToken ct);

        /// <summary>
        /// ottiene un nome utente partendo da un id 
        /// </summary>
        /// <returns>stringa con username</returns>
        Task<string> GetUsernameByIdAsync(int id, CancellationToken ct);

        Task<bool> DeleteUserByIdAsync(int id, CancellationToken ct);
        

        Task<bool> UpdateUserAsync(UserDTO utente, CancellationToken ct);



        //============================ LATO QUIZ =============================================


        /// <summary>
        /// ottiene una lista di tutti i quizDTO pubblici e/o appartenenti all'userId passato
        /// </summary>
        /// <returns>una lista di quizDTO pubblici e/o appartenenti all'userId passato</returns>//TODO: avrebbe senso recuperarne solo un tot
        Task<List<QuizDTO>> GetAllPublicQuizzesAsync(int?userId,CancellationToken ct);


        /// <summary>
        /// aggiunge un quiz al database
        /// </summary>
        /// <returns>id del quiz in caso di successo, altrimenti null</returns>
        Task<int?> CreateQuizAsync(QuizDTO request, CancellationToken ct);

        /// <summary>
        /// controlla se un quiz esite o meno dato il suo presunto id
        /// </summary>
        /// <returns>true se esiste, false altrimenti</returns>
        Task<bool> QuizExistByIdAsync(int id, CancellationToken ct);

        /// <summary>
        /// query fatta su misura per sapere se un quiz esiste o meno e se è pubblico. 
        /// <br/>cerca dentro quiz il quizId e se lo trova prende userId e il valore di pubblico
        /// <br/>li restituisce in un oggetto QuizPublicDTO
        /// </summary>
        /// <returns>un oggetto QuizPublicDTO o null  </returns>
        Task<QuizPublicDTO?> GetQuizPublicDataByIdAsync(int id, CancellationToken ct);

        /// <summary>
        /// ottiene l'userId associato ad un determinato quizId
        /// </summary>
        /// <returns>userId trovato o null</returns>
        Task<int?> GetQuizUserIdByIdAsync(int id, CancellationToken ct);
        /// <summary>
        /// controlla se un quiz ha domande o meno e torna un bool
        /// </summary>
        /// <returns>true se ha domande, false altrimenti</returns>
        Task<bool> QuizHaveQuestionsAsync(int id, CancellationToken ct);

        Task<bool> UpdateQuizAsync(QuizDTO request, CancellationToken ct);

        Task<bool> DeleteQuizByIdAsync(int id, CancellationToken ct);

        Task<bool> MakeQuizPrivate(int quizId, int userId, CancellationToken ct);

        //============================= LATO DOMANDE ==========================


        /// <summary>
        /// ottiene le domande di un quiz dato il suo id
        /// </summary>
        /// <returns>un QuestionsDTO con le domande</returns>
        Task<QuestionsDTO> GetQuestionsByQuizIdAsync(int quizId, CancellationToken ct);

        /// <summary>
        /// aggiunge delle domande ad un quiz nel database dato un QuestionsDTO
        /// </summary>
        /// <returns>la lista di id delle domande aggiunte</returns>
        Task<List<int>> AddQuestionsToQuizAsync(QuestionsDTO request, CancellationToken ct);


        Task<bool> DeleteQuestionsByIdsAsync(List<int> ids, CancellationToken ct);


        Task<bool> UpdateQuestionsAsync(QuestionsDTO request, CancellationToken ct);

        Task<int?> CountQuestionsByQuizIdAsync(int quizId, CancellationToken ct); 


        //============================= LATO QUIZSEED ==========================

        /// <summary>
        /// aggiunge un nuovo quizSeed al db dato un suo QuizSeedDTO
        /// </summary>
        /// <returns></returns>
        Task<int?> CreateQuizSeedAsync(QuizSeedDTO request, CancellationToken ct);


        /// <summary>
        /// ottiene tutti i quizSeed di un relativo quiz che sono pubblici e/o appartenenti all'userId passato
        /// </summary>
        /// <returns></returns>
        Task<List<QuizSeedDTO>> GetQuizSeedsByQuizIdAndUserIdAsync(int id,int? userId, CancellationToken ct);


        Task<bool> UpdateQuizSeedAndDeleteScoreboardRecordsByIdAsync(int id , CancellationToken ct);


        Task<bool> DeleteQuizSeedAndDeleteScoreboardRecordsByIdAsync(int id, CancellationToken ct);

        Task<bool> MakeQuizSeedPrivate(int id, CancellationToken ct);

        //============================= LATO SCOREBOARD ==========================

        Task<int?> CreateScoreboardRecord(ScoreboardDTO request, CancellationToken ct);


        //============================= LATO PULL =============================




    }
}
