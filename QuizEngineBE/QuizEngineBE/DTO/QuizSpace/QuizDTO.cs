using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO.QuizSpace
{
    public class QuizDTO
    {
        public string Name { get; set; } = null!;
        public int? Id { get; set; }
        public Dictionary<string, int>? Difficulties { get; set; }
        public string? DifficultValues { get; set; }
        public int UserId { get; set; }
        public bool Public { get; set; } = false;

        [JsonIgnore]
        public bool CheckFields =>
            
            !string.IsNullOrWhiteSpace(Name)// Il nome deve sempre esserci            
            && UserId >= 0// UserId deve essere valido           
            && (Id == null || Id >= 0)// Id del quiz se presente deve essere valido           
            && (DifficultValues == null || !string.IsNullOrWhiteSpace(DifficultValues)) // DifficultValues se presente non deve essere solo spazi bianchi            
            && (Difficulties == null || Difficulties.Values.All(v => v > 0));// Difficulties se presente deve avere tutti valori >0
    }
}
