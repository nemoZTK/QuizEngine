using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO
{
    public class QuizDTO
    {

        public string Name { get; set; } = null!;

        public Dictionary<string, int>? Difficulties { get; set; }

        public string? DifficultValues { get; set; }

        public int UserId { get; set; }

        public bool Pubblico { get; set; } = false;


        [JsonIgnore]
        public bool CheckFields =>
                   !string.IsNullOrWhiteSpace(Name) &&       
                    UserId > 0 &&                             
                   (Difficulties == null || Difficulties.Values.All(v => v > 0));



    }
}
