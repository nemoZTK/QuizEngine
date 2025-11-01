using System.Text.Json.Serialization;

namespace QuizEngineBE.DTO.PullSeedSpace
{
    public class QuizSeedDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; } = null!;
        public int QuizId { get; set; }
        public int UserId { get; set; }
        public int QuestionNumner { get; set; }
        public string Mode { get; set; } = null!;
        public bool Public { get; set; }
        public int? TotalTime { get; set; }
        public bool? QuestionTotalTime { get; set; }
        public bool? CanGoBack { get; set; }
        public bool? CanUseTimeGap { get; set; }

        [JsonIgnore]
        public bool CheckFields =>
                    !string.IsNullOrWhiteSpace(Name) &&
                    QuizId > 0 && QuestionNumner > 0 &&
                    Modes.Contains(Mode) && UserId > 0 &&
                    (TotalTime is null || TotalTime > 1);
    }
}
