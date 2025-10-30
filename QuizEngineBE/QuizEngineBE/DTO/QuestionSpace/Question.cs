using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO.QuestionSpace
{
    public class Question
    {

        public string Text { get; set; } = null!;
        public List<string> RightAnswers { get; set; } = null!;
        public List<string> WrongAnswers { get; set; } = null!;
        public string? WrongAnswersString { get; set; }
        public string? RightAnswersString { get; set; }
        public int? Time { get; set; }
        public string? Difficulty { get; set; }
        public string? Sequence { get; set; }
        public int? SequenceNumber { get; set; }
        public string? Variant { get; set; }

        /// <summary>
        /// Controlla se i campi principali della domanda sono validi.
        /// </summary>
        [JsonIgnore]
        public bool CheckFields =>
            // obbligatori
            !string.IsNullOrWhiteSpace(Text) &&
            RightAnswers != null && RightAnswers.Count > 0 &&
            WrongAnswers != null && WrongAnswers.Count > 0 &&
            // opzionali validi se presenti
            (RightAnswersString == null || !string.IsNullOrWhiteSpace(RightAnswersString)) &&
            (WrongAnswersString == null || !string.IsNullOrWhiteSpace(WrongAnswersString)) &&
            (Time == null || Time >= 0) &&
            (Sequence == null || !string.IsNullOrWhiteSpace(Sequence)) &&
            (SequenceNumber == null || SequenceNumber >= 0) &&
            (Variant == null || !string.IsNullOrWhiteSpace(Variant));

    }
}
