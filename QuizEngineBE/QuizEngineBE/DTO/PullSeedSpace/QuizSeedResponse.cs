namespace QuizEngineBE.DTO.PullSeedSpace
{
    public class QuizSeedResponse : ResponseBase<QuizSeedResponse>
    {
        public int? Id { get; set; }

        public List<QuizSeedDTO>? QuizSeeds { get; set; }
    }
}
