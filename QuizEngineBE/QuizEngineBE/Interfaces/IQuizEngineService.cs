using QuizEngineBE.DTO.PullSeedSpace;
using QuizEngineBE.DTO.QuestionSpace;
using QuizEngineBE.DTO.QuizSpace;
using QuizEngineBE.DTO.ScoreboardSpace;
using QuizEngineBE.DTO.UserSpace;

namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// service principale che orchestra tutti gli altri, conosce solo chi chiamare per fare cosa,
    /// <br/>non contiene logica se non per l'autorizzazione e i return
    /// <br/> usa QuizService, QuizSeedService, PullSeedService, UserService, SessionService, RandomizerService e ParseService
    /// </summary>
    public interface IQuizEngineService
    {

        //============================ LATO QUIZ ==================================


        /// <summary>
        /// aggiunge un quiz dato un QuizDTO ed il token dell'utente
        /// </summary>
        /// <returns>un oggetto QuizResponse</returns>
        Task<QuizResponse> CreateQuiz(QuizDTO quiz, string? token);

        /// <summary>
        /// ottiene un quiz dato il suo id,
        /// <br/>se il quiz esiste ed è pubblico o appartenente allo userId allora viene restituito (con le sue domande annesse)
        /// </summary>
        /// <returns>un oggetto QuizResponse</returns>
        Task<QuizResponse> GetQuizById(int id, int? userId, string? token);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<QuizResponse> UpdateQuiz(QuizDTO quiz, string? token);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<QuizResponse> DeleteQuiz(int id);




        //============================= LATO DOMANDE =====================================

        /// <summary>
        /// aggiunge domande ad un quiz esistente dato un QuestionsDTO ed il token dell'utente 
        /// </summary>
        /// <returns>un oggetto QuestionsResponse</returns>
        Task<QuestionsResponse> AddQuestionsToQuiz(QuestionsDTO domande, string? token);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<QuestionsResponse> UpdateQuestions(QuestionsDTO questions, string? token);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<QuestionsResponse> DeleteQuestions(QuestionsDTO questions, string? token);

        //============================= LATO QUIZSEED ============================

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<QuizSeedResponse> CreateQuizSeed(QuizSeedDTO quizSeedDTO, string? token);

        //============================= LATO SCOREBOARD =============================

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<ScoreboardResponse> GetScoreboardByQuizSeedIdAndUserId(int id,int?userId,string? token);




        //============================== LATO UTENTE =======================

        /// <summary>
        /// registra un nuovo utente dato un UserDTO
        /// </summary>
        /// <returns>una UserResponse</returns>
        Task<UserResponse> RegisterUser(UserDTO user);

        /// <summary>
        /// effettua il login di un utente data una LogOnRequest
        /// </summary>
        /// <returns>un oggetto UserResponse</returns>
        Task<UserResponse> DoLogin(LogOnRequest loginRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<UserResponse> UpdateUserInfo(UserDTO user, string? token);
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<UserResponse> DeleteUser(string id,string? token);





    }
}
