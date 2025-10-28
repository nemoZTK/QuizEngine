using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO
{
    public class QuestionsDTO
    {
        public int QuizId { get; set; }

        public List<Question> Questions { get; set; } = null!;

        public List<int>? QuestionsId { get; set; } = null!;


        public int UserId { get; set; }



        [JsonIgnore]
        public bool CheckFields =>
            QuizId > 0 &&                              // quizId obbligatorio
            UserId > 0 &&                             // userId obbligatorio
            Questions != null &&                     // Questions obbligatorio
            Questions.Count > 0 &&                  // almeno una domanda
            Questions.All(q => q.CheckFields);     // tutte le domande devono essere valide



    }
}
