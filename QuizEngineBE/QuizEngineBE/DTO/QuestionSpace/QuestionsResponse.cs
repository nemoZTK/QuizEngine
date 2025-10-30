using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO.QuestionSpace
{
    /// <summary>
    /// la question response è l'oggetto di risposta standard per le chiamate che coinvolgono la singola domanda
    /// <br/> contiene anche risposte standard come WrongDifficulties
    /// </summary>
    public class QuestionsResponse : ResponseBase<QuestionsResponse>
    {
        public List<int> QuestionsID { get; set; } = null!;



        public QuestionsResponse WrongDifficulties (string difficolta) => new()
        {
            Success = false,
            Message = "difficoltà non esistente -->"+ difficolta
        };


    }
}
