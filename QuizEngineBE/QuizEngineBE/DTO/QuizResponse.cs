using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO
{
    public class QuizResponse : ResponseBase<QuizResponse>
    {
        public int Id { get; set; }



        [JsonIgnore]
        public QuizResponse WrongFields => new()
        {
            Success = false,
            Message = "nome o id utente mancanti/errati"
        };



    }
}
