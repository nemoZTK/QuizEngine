using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO
{
    /// <summary>
    /// la quiz response può contenere un quiz completo con domande annesse,
    /// <br/>è l'oggetto standard per risposte che coinvolgono un quiz
    /// <br/> contiene sovrascrive anche risposte standard come WrongFields
    /// </summary>
    public class QuizResponse : ResponseBase<QuizResponse>
    {
        public int? Id { get; set; }

        public string? Name { get; set; }


        public Dictionary<string, int>? Difficulties { get; set; }

        public List<Question>? Questions { get; set; }






    }
}
