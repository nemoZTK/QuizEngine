using QuizEngineBE.DTO.QuestionSpace;
using QuizEngineBE.DTO.QuizSpace;

namespace QuizEngineBE.Interfaces
{
    /// <summary>
    /// service che si occupa dell'ottenimento, inserimento, modifica ed eliminazione di quiz e domande
    /// <br/> usa DbService 
    /// </summary>
    public interface IQuizService
    {

        //============================= LATO QUIZ ===============================

        /// <summary>
        /// Recupera un quiz dato il suo ID ,
        /// <br/>se è pubblico o appartenente all'user id che si può passare allora viene restituito
        /// </summary>
        /// <returns>Un oggetto QuizResponse con i dati del quiz.</returns>
        Task<QuizResponse> GetQuizById(int id, int? userId);

        /// <summary>
        /// Aggiunge Un QuizSpace
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<QuizResponse> CreateQuiz(QuizDTO request);


        Task<QuizResponse> UpdateQuiz(QuizDTO request);


        Task<QuizResponse> DeleteQuiz(int id);

        //============================= LATO DOMANDE ==========================================

        /// <summary>
        /// Aggiunge una lista di domande a un quiz esistente.
        /// </summary>
        /// <returns>Un oggetto QuestionsResponse con l’esito dell’operazione.</returns>
        Task<QuestionsResponse> AddQuestionsToQuiz(QuestionsDTO request);

        Task<QuestionsResponse> UpdateQuestions(QuestionsDTO request);


        Task<QuizResponse> DeleteQuestions(List<int> ids);





    }
}
