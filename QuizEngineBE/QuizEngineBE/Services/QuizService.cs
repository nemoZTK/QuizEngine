using Azure;
using Azure.Core;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using QuizEngineBE.DTO;
using QuizEngineBE.DTO.QuestionSpace;
using QuizEngineBE.DTO.QuizSpace;
using QuizEngineBE.Interfaces;
using QuizEngineBE.Models;
using System.Data;

namespace QuizEngineBE.Services
{
    public class QuizService(DbService db) : IQuizService
    {

        private readonly DbService _dbServ= db;

        //====================================== M E T O D I == P R I V A T I ===========================================

        /// <summary>
        /// Parsa un dizionario e lo trasforma in una stringa "string=int;string=int"
        /// </summary>
        /// <returns>Una stringa parsata "string=int;string=int"</returns>
        private readonly Func<Dictionary<string, int> ,string> ParseDictionary = dict => string.Join(";", dict.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        
        
        /// <summary>
        /// Parsa una stringa "string=int;string=int"  e la trasfotma in un dizionario
        /// </summary>
        /// <returns>Un dizionario string,int </returns>
        private readonly Func<string,Dictionary<string, int>> ParseString= str =>
        
            string.IsNullOrWhiteSpace(str) ? [] :           // controllo se è vuota in caso ritorno subito
                                                                                      // un oggetto vuoto
                str.Split(';', StringSplitOptions.RemoveEmptyEntries)                // altrimenti splitto la stringa per ogni blocco ;
                                                                                    // recupero chiave e valore
                   .Select(p => p.Split('=', 2))                                   // splittandoli per =  ( chiave = valore ) 
                                                                                  // filtro solo le coppie valide.
                   .Where(kv => kv.Length == 2 && int.TryParse(kv[1], out _))    // poi trasformo l'enumerable in un dizionario, 
                                                                                // convertendo il valore da string a int
                   .ToDictionary(kv => kv[0], kv => int.Parse(kv[1]))          
        ;


        /// <summary>
        /// Parsa una stringa "string@#%@string" in una lista
        /// </summary>
        ///<returns>Una lista di strings </returns>
        private readonly Func<List<string>, string> ParseAnswerListToString = list => string.Join("@#%@", list);

        /// <summary>
        /// Parsa una lista di strings in una stringa "string@#%@string"
        /// </summary>
        ///<returns>Una stringa parsata "string@#%@string"</returns>
        private readonly Func<string, List<string>> ParseAnswerStringToList = s => [.. s.Split("@#%@", StringSplitOptions.None)];

        /// <summary>
        /// Parsa risposte giuste e sbagliate da list a string
        /// </summary>
        /// <returns>L'oggetto in entrata, con le stringhe valorizzate in caso ci fossero valori</returns>
        private  List<Question> ParseAnswersToString(List<Question> questions)
        {
            foreach (Question q in questions)
            {
                if (q.WrongAnswers.Count > 0 && q.RightAnswers.Count > 0)
                {
                    q.WrongAnswersString = ParseAnswerListToString(q.WrongAnswers);
                    q.RightAnswersString = ParseAnswerListToString(q.RightAnswers);
                    q.WrongAnswers = [];
                    q.RightAnswers = [];
                }
            }

            return questions;
        }

        /// <summary>
        /// Parsa risposte giuste e sbagliate da string a list
        /// </summary>
        /// <returns>L'oggetto in entrata, con le liste valorizzate in caso le stringhe fossero valide</returns>
        private List<Question> ParseAnswersToList(List<Question> questions)
        {
            foreach (Question q in questions)
            {
                if (!string.IsNullOrWhiteSpace(q.WrongAnswersString) && !string.IsNullOrWhiteSpace(q.RightAnswersString))
                {
                    q.WrongAnswers = ParseAnswerStringToList(q.WrongAnswersString??"");
                    q.RightAnswers = ParseAnswerStringToList(q.RightAnswersString??"");
                    q.WrongAnswersString = null;
                    q.RightAnswersString = null;
                }

            }

            return questions;
        }

        /// <summary>
        /// Controlla che le difficoltà di ogni domanda della lista sia una delle difficoltà del quiz
        /// <br/>per farlo parsa la stringa delle difficultValues in un dizionario e poi controlla le chiavi, 
        /// <br/>se torova una difficoltà non presente la restituisce un WrongDifficulties(missingDifficulty)
        /// </summary>
        /// <returns>un bool</returns>
        private QuestionsResponse DifficultiesCheck(List<Question> domande, string difficultValues)
        {
            QuestionsResponse response = new();


            Dictionary<string, int> difficulties = ParseString(difficultValues);

            string? missingDifficulty = domande
                .Select(d => d.Difficulty)
                .FirstOrDefault(d => string.IsNullOrWhiteSpace(d) || !difficulties.ContainsKey(d));

            if (missingDifficulty != null)
                return response.WrongDifficulties(missingDifficulty);

            response.Success = true;
            return response;
        }



        //====================================== M E T O D I == P U B B L I C I ===========================================


        //============================= LATO QUIZ =======================
        public async Task<QuizResponse> CreateQuiz(QuizDTO request)
        {
           QuizResponse response = new();
            if (!request.CheckFields) return response.MissingFields();

            if (request.Difficulties != null) request.DifficultValues = ParseDictionary(request.Difficulties);
            
            
            response.Id= await _dbServ.CreateQuizAsync(request);
            response.Success = true;
            return response;
        }

        public async Task<T> CanSeeQuiz<T>(T response, int id, int? userId) where T : ResponseBase<T>
        {
            var info = await _dbServ.GetQuizPublicDataByIdAsync(id);

            if (info is null)
                return response.IdNotFound;

            if (!info.Pubblico && info.UserId != userId)
                return response.IdNotFound;

            response.Success = true;
            return response;
        }



        public async Task<QuizResponse> GetQuizById(int id,int? userId)
        {
            QuizResponse response = new();

            response = await CanSeeQuiz(response, id, userId);

            if (!response.Success) return response;
            
            QuizDTO quiz = await _dbServ.GetQuizByIdAsync(id);
            response.Id = quiz.Id;
            response.Name = quiz.Name;  

            if (!quiz.CheckFields)
                return response.WrongFields();

            if (!quiz.DifficultValues.IsNullOrEmpty())
                response.Difficulties = ParseString(quiz.DifficultValues??"");
            
            if (await _dbServ.QuizHaveQuestionsAsync(id))
            {
                QuestionsDTO questions = await _dbServ.GetQuestionsByQuizIdAsync(id);

                response.Questions = ParseAnswersToList(questions.Questions);
            }

            return response;
         }

        public Task<QuizResponse> UpdateQuiz(QuizDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<QuizResponse> DeleteQuiz(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<int, string>> GetPublicQuzNames(int userId)
        {
            throw new NotImplementedException();
        }


        //============================= LATO DOMANDE ============================

        public async Task<QuestionsResponse> AddQuestionsToQuiz(QuestionsDTO request)
        { 
            QuestionsResponse response = new();
            if (!request.CheckFields) return response.MissingFields();
            QuizDTO quiz= await _dbServ.GetQuizByIdAsync(request.QuizId);
            if (!string.IsNullOrWhiteSpace(quiz.DifficultValues))
            {
                response=DifficultiesCheck(request.Questions, quiz.DifficultValues);
                if (!response.Success) return response;
            }
            request.Questions = ParseAnswersToString(request.Questions);
            response.QuestionsID = await _dbServ.AddQuestionsToQuizAsync(request);
            
            if(response.QuestionsID.Count> 0) response.Success = true;

            return response;

            
        }
        public Task<QuestionsResponse> UpdateQuestions(QuestionsDTO request)
        {
            throw new NotImplementedException();
        }

        public Task<QuizResponse> DeleteQuestions(List<int> ids)
        {
            throw new NotImplementedException();
        }


        public Task<int?> GetQuestionNumber(int quizId) => _dbServ.CountQuestionsByQuizIdAsync(quizId);
    }
}
